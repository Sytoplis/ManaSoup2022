using System.Collections.Generic;
using UnityEngine;

public partial class MapGenerator {
    public class MapPostprocessor {
        public Array2D<FlipSegment> map;
        public readonly FlipSegment[] flipSegments;
        public readonly Socket[] noColSockets;//connectionSockets
        private List<List<int>> sections;

        public MapPostprocessor(Array2D<FlipSegment> map, FlipSegment[] flipSegments, Socket[] noColSockets) {
            this.map = map;//dont create a copy -> modify the original
            this.flipSegments = flipSegments;
            this.noColSockets = noColSockets;
            FindSections();//initialize sections
        }

        public void PostProcess() {
            FindAndUseConnections(FindNeighbours(out int maxIter), maxIter);
        }

        #region Find Sections
        public void FindSections() {
            HashSet<int> visited = new HashSet<int>();
            for (int i = 0; i < map.Length; i++) {
                if (map[i].IsEmpty())
                    visited.Add(i);//add all empty tiles as visited
            }

            sections = new List<List<int>>();
            bool newSectionFound;
            do {
                newSectionFound = GetSection(ref visited, out List<int> section);
                if (newSectionFound)
                    sections.Add(section);
            } while (newSectionFound);
        }


        private bool GetUnvisited(in HashSet<int> visited, out int unvisited) {
            for (unvisited = 0; unvisited <= map.Length; unvisited++) {
                if (!visited.Contains(unvisited))
                    break;
            }
            return unvisited != map.Length;//return if an unvisited tile was found
        }

        private bool GetSection(ref HashSet<int> visited, out List<int> section) {
            bool hasUnvisized = GetUnvisited(in visited, out int init);
            if (!hasUnvisized) {
                section = null;
                return false;
            }


            //use DFS to find the entire section around "init"
            section = new List<int>();
            Stack<int> toVisit = new Stack<int>();
            toVisit.Push(init);

            while (toVisit.Count > 0) {
                int i = toVisit.Pop();
                if (visited.Contains(i))
                    continue;

                section.Add(i);
                visited.Add(i);
                Vector2Int pos = map.GetPos(i);

                //Add all connected tiles
                for (int d = 0; d < DirExt.directions.Length; d++) {
                    Vector2Int neighbour = pos + DirExt.directions[d];
                    if (!map.InBounds(neighbour)) continue;

                    if (map[i].GetSocket(d).isCollision) continue;//only add connected

                    int _i = map.GetIndex(neighbour);
                    if (visited.Contains(_i)) continue;//only add not visited

                    toVisit.Push(_i);
                    section.Add(_i);
                }
            }
            return true;
        }
        #endregion



        #region Fix Sections

        public List<(int, int)>[] FindNeighbours(out int maxIter) {
            maxIter = 0;

            //------------ FIND ALL NEIGHBOURS OF EACH SECTION -------------------
            List<(int, int)>[] neighbours = new List<(int, int)>[sections.Count];
            for (int s = 0; s < sections.Count; s++) {
                neighbours[s] = new List<(int, int)>();
                for (int sec_i = 0; sec_i < sections[s].Count; sec_i++) {
                    int i = sections[s][sec_i];
                    Vector2Int pos = map.GetPos(i);

                    for (int d = 0; d < DirExt.directions.Length; d++) {
                        Vector2Int neighbour = pos + DirExt.directions[d];
                        if (!map.InBounds(neighbour)) continue;

                        int _i = map.GetIndex(neighbour);
                        if (map[_i].IsEmpty()) continue;//ignore empty tiles
                        if (sections[s].Contains(_i)) continue;//only add tiles from other sections

                        neighbours[s].Add((i, _i));//add neighbours from different sections
                        maxIter++;
                    }
                }
            }
            return neighbours;
        }

        public void FindAndUseConnections(List<(int, int)>[] neighbours, int maxIter) {
            List<(int, int)> mainNeighbours = new List<(int, int)>();
            HashSet<int> mainSection = new HashSet<int>();
            HashSet<int> addedSections = new HashSet<int>();
            mainSection.UnionWith(sections[0]);//add the first section as initial section
            addedSections.Add(0);
            mainNeighbours.AddRange(neighbours[0]);
            for(int iter = 0; iter < maxIter; iter++) 
            {
                if (mainNeighbours.Count == 0) {
                    Debug.LogError("Not all Sections could be connected");
                    break;
                }

                int self, neighbour;
                {//FIND A NEIGHBOUR AND CONNECT TO IT
                    //pick a random neighbour from all mainsections
                    int nIndex = Random.Range(0, mainNeighbours.Count);
                    (self, neighbour) = mainNeighbours[nIndex];

                    //remove and repeat, if it leads to the main section again
                    if (mainSection.Contains(neighbour)) {
                        mainNeighbours.RemoveAt(nIndex);
                        continue;
                    }

                    bool wasConnected = Connect(self, neighbour);
                    if (!wasConnected) {//if the connection failed somehow
                        mainNeighbours.RemoveAt(nIndex);//remove the not possible connection
                        continue;
                    }
                }


                {//ON SUCCESSFUL CONNECTION
                    //add new section to main section
                    int otherSection = GetSection(neighbour, sections);//SLOW
                    mainSection.UnionWith(sections[otherSection]);
                    addedSections.Add(otherSection);
                    mainNeighbours.AddRange(neighbours[otherSection]);

                    if (addedSections.Count == sections.Count) {
                        Debug.Log("All connections found successfully");
                        break;
                    }
                }
            }
        }

        public bool Connect(int a, int b) {
            Vector2Int vecA = map.GetPos(a);
            Vector2Int vecB = map.GetPos(b);

            //Specify the sockets INCOMING from the surounding tiles
            Socket2D suroundingA = GetSuroundingSockets(vecA);
            Socket2D suroundingB = GetSuroundingSockets(vecB);

            Vector2Int A_ConVec = vecB - vecA;//direction from A to B
            if (A_ConVec.sqrMagnitude != 1) {
                Debug.LogError("Connectiondistance is too big");
                return false;
            }

            Direction A_ConDir = DirExt.ToDir(A_ConVec);


            //Find a tile, which sockets fit to the specified sockets while making sure that the connection socket is a connection

            //First go through all sockets and find one that fits for A
            if (!FindFittingSegment(out FlipSegment segA, ref suroundingA, (int)A_ConDir, noColSockets, out Socket B_Socket))
                return false;
            map[a] = segA;

            //then use that socket to find one for B as well
            suroundingB.SetSocket((int)A_ConDir.InvertDir(), B_Socket);
            if (!FindFittingSegment(out FlipSegment segB, suroundingB))
                return false;
            map[b] = segB;
            return true;
        }


        //tries to find the section a given tile [i] is in
        public static int GetSection(int i, List<List<int>> sections) {
            for (int s = 0; s < sections.Count; s++) {
                if (sections[s].Contains(i))
                    return s;
            }
            Debug.LogWarning($"Section for index {i} not found");
            return 0;
        }
        #endregion


        private Socket2D GetSuroundingSockets(Vector2Int pos) {
            Socket2D s3d = new Socket2D();
            for (int d = 0; d < DirExt.directions.Length; d++) {
                Vector2Int nPos = pos + DirExt.directions[d];
                Direction connectionDir = ((Direction)d).InvertDir();//direction facing to this tile from the other tile
                s3d.SetSocket(d, map[nPos].GetSocket(connectionDir));
            }
            return s3d;
        }


        private bool FindFittingSegment(out FlipSegment segment, ref Socket2D surounding, int d, Socket[] possibleConnections, out Socket connector) {
            for (int c = 0; c < possibleConnections.Length; c++) {
                connector = possibleConnections[c];
                surounding.SetSocket(d, connector);
                for (int seg = 0; seg < flipSegments.Length; seg++) {
                    if (flipSegments[seg].SocketFits(surounding)) {
                        segment = flipSegments[seg];
                        return true;
                    }
                }
            }

            segment = null;
            connector = null;
            Debug.LogError($"No fitting segment found for {surounding} with all possible connection Sockets");
            return false;
        }
        private bool FindFittingSegment(out FlipSegment segment, Socket2D surounding) {
            for (int s = 0; s < flipSegments.Length; s++) {
                if (flipSegments[s].SocketFits(surounding)) {
                    segment = flipSegments[s];
                    return true; 
                }
                   
            }

            segment = null;
            Debug.LogError($"No fitting segment found for {surounding}");
            return false;
        }

    }

}

