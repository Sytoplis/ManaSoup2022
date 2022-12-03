using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Vector2Int mapSize = Vector2Int.one * 5;
    public MapSegment[] segments;

    public Array2D<FlipSegment> map;
    private Array2D<PossibilityTile> grid;//FOR DEBUGGING ONLY


    private void Start() {
        GenerateMap();
    }


    #region Initial Computation
    private FlipSegment[] GenerateFlipSegments(out float weightSum) {//generate TurnSegments from the mapSegment
        weightSum = 0;

        List<FlipSegment> flipped = new List<FlipSegment>();
        for (int i = 0; i < segments.Length; i++) {
            bool flip = segments[i].flippable;
            float weightMul = flip ? 1.0f : 2.0f;

            //add the unflipped segment
            flipped.Add(new FlipSegment(segments[i], false, weightMul));
            weightSum += weightMul * segments[i].weight;

            if (flip) {//if flip: also add the flipped version
                flipped.Add(new FlipSegment(segments[i], true, weightMul));
                weightSum += weightMul * segments[i].weight;
            }
        }
        return flipped.ToArray();
    }

    private void FindAllPossibleSockets(out List<Socket>[] basePossibilities, in FlipSegment[] flipSegments) {
        //go through all tiles and find which sockets they have and add them
        basePossibilities = new List<Socket>[4];
        for (int d = 0; d < 4; d++) {
            basePossibilities[d] = new List<Socket>();
            for (int s = 0; s < flipSegments.Length; s++) {
                if (!basePossibilities[d].Contains(flipSegments[s].GetSocket(d)))
                    basePossibilities[d].Add(flipSegments[s].GetSocket(d));
            }
        }
    }
    #endregion

    public void GenerateMap() {
        //---------------- INIT -----------------
        FlipSegment[] flipSegments = GenerateFlipSegments(out float weightSum);
        FindAllPossibleSockets(out List<Socket>[] basePossibilities, in flipSegments);

        grid = new Array2D<PossibilityTile>(mapSize);
        for (int i = 0; i < grid.Length; i++)
            grid[i] = new PossibilityTile(in flipSegments, weightSum);

        //--------------- GENERATION -------------
        GenerateConnectionCollisionDFS(ref grid, in basePossibilities, Vector2Int.zero);
        GenerateConnection(ref grid, flipSegments.Length);

        CreateMap(ref grid);
        //TODO: place the rooms on the tilemap
    }

    /// <summary>
    /// generate connection using DFS
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="possibleSockets">this array contains the initial possibilities for all direction</param>
    private void GenerateConnectionCollisionDFS(ref Array2D<PossibilityTile> grid, in List<Socket>[] possibleSockets, Vector2Int startPos) {
        Array2D<bool> visited = new Array2D<bool>(grid.size);
        Stack<(Vector2Int, Vector2Int)> toVisit = new Stack<(Vector2Int, Vector2Int)>();
        toVisit.Push((startPos, startPos));//push the starting tile
        while(toVisit.Count > 0) {
            (Vector2Int lastPos, Vector2Int pos) = toVisit.Pop();
            if (!visited[pos]) {//if the tile was not visited
                visited[pos] = true;//visit the tile
                Vector2Int dir = pos - lastPos;
                if (dir.sqrMagnitude == 1) {//only connect to the one before, if the connection is valid

                    //connect tile to the one before
                    Direction lastToCurrent = DirExt.ToDir(dir);
                    Direction CurrentToLast = lastToCurrent.InvertDir();
                    grid[pos    ].SetPossibleSockets((int)CurrentToLast, possibleSockets[(int)CurrentToLast]);
                    grid[lastPos].SetPossibleSockets((int)lastToCurrent, possibleSockets[(int)lastToCurrent]);
                }

                for (int d = 0; d < 4; d++) {//TODO: randomize the DFS
                    if (!grid.InBounds(pos + DirExt.directions[d]))
                        continue;

                    if (grid[pos + DirExt.directions[d]] == null)
                        toVisit.Push((pos, pos + DirExt.directions[d]));
                }
            }
        }
    }



    #region Wavefunction Collapse

    bool LowestEntropyCollapse(ref Array2D<PossibilityTile> grid, int flipSegmentCount) {//collapse the box with the smallest number of possibilities
        //Get all the GridBoxes with the lowest Entropy
        int min = flipSegmentCount + 1;
        List<int> minGrids = new List<int>();
        for (int i = 0; i < grid.Length; i++) {
            if (grid[i].possibilities.Count <= 1 || grid[i].possibilities.Count > min) continue;

            if (grid[i].possibilities.Count < min) {
                min = grid[i].possibilities.Count;
                minGrids.Clear();
            }
            minGrids.Add(i);//add every tile that has min possibilities
        }


        if (min == flipSegmentCount + 1 || minGrids.Count == 0)//if everything is already collapsed
            return false;

        int toCollapse = minGrids[UnityExtensions.GetRndm(minGrids.Count)];
        bool collapseSuccess = CollapseRndm(ref grid, toCollapse);
        if (!collapseSuccess)
            Debug.Log("Collapse error");
        return collapseSuccess;
    }

    bool CollapseRndm(ref Array2D<PossibilityTile> grid, int toCollapse) {//tries to collapse a gridbox. returns if successful
        int collapseSegment = grid[toCollapse].GetWeightedRnd();
        return Collapse(ref grid, toCollapse, grid[toCollapse].possibilities[collapseSegment]);//pick one segment of all possible segments
    }
    bool Collapse(ref Array2D<PossibilityTile> grid, int toCollapse, FlipSegment segment) {//tries to collapse a gridbox. returns if successful
        if (grid[toCollapse].SetResult(segment)) {
            PropergateCollapse(ref grid, toCollapse);
            return true;
        }
        return false;
    }
    void ForceCollapse(ref Array2D<PossibilityTile> grid, int toCollapse, FlipSegment segment) {
        grid[toCollapse].ForceResult(segment);
        PropergateCollapse(ref grid, toCollapse);
    }


    void PropergateCollapse(ref Array2D<PossibilityTile> grid, int init) {
        Stack<int> propergateStack = new Stack<int>();
        propergateStack.Push(init);

        //while (toPropergate.Count > 0) {
        for (int iter = 0; iter < 6 * grid.Length; iter++) {//ONLY FOR DEBUGGING -> do not get STUCK in a while loop
            if (propergateStack.Count <= 0) break;

            int toPropergate = propergateStack.Pop();
            Vector2Int pos = grid.GetPos(toPropergate);

            for (int d = 0; d < DirExt.directions.Length; d++) {
                Vector2Int neighbour = pos + DirExt.directions[d];
                if (!grid.InBounds(neighbour)) continue;

                int propergationNeighbour = grid.GetIndex(neighbour);
                //if (grid[propergationNeighbour].possibilities.Count <= 1) continue;

                //only allow sockets that can connect to the possible sockets on the other side
                HashSet<Socket> sockets = grid[toPropergate].possibleSockets[d];
                int changeCount = grid[propergationNeighbour].OnlyAllow(sockets, (Direction)DirExt.InvertDir(d));

                if (changeCount > 0 && !propergateStack.Contains(propergationNeighbour))//if grid[propergationNeighbour] has a change in possibilities
                    propergateStack.Push(propergationNeighbour);//propergate this change
            }
        }

        if (propergateStack.Count > 0)
            Debug.LogError("Propergation took to many iterations");
    }


    private void GenerateConnection(ref Array2D<PossibilityTile> grid, int flipSegmentCount) {//generate connection type using WF collapse
        //start by deleting all socket that dont have a fitting segment
        for (int i = 0; i < grid.Length; i++)
            grid[i].RemoveInvalidTiles();


        //----------- WAVE FUNCTION COLLAPSE --------------------
        //generate connection type using WF collapse
        int iterLimit = grid.Length;
        //int iterLimit = 20;
        for (int iter = 0; iter < iterLimit; iter++) {
            if (!LowestEntropyCollapse(ref grid, flipSegmentCount)) {//keeps collapsing until everything is collapsed
                Debug.Log("Collapse is done");
                break;
            }
        }
    }

    #endregion


    private void CreateMap(ref Array2D<PossibilityTile> grid) {
        map = new Array2D<FlipSegment>(grid.size);
        for(int i = 0; i < grid.Length; i++) {
            if (grid[i].possibilities.Count != 1) {
                Debug.LogError(grid.GetPos(i) + "doesnt have one possibility");
            }
            map[i] = grid[i].possibilities[0];
        }
    }


    #region Gizmos for Debugging
    private void OnDrawGizmos() {
        if (map != null) {
            for (int i = 0; i < map.Length; i++) {
                Vector2Int pos = map.GetPos(i);
                if (pos.y != 1) continue;

                //if (IsEmpty(map[i])) continue;//skip empty

                //DrawGizmosSegmentPSockets(pos + Vector3.up, 0.5f, grid[i]);
                for (int j = 0; j < grid[i].possibilities.Count; j++) {
                    DrawGizmosSegment((Vector3Int)pos + Vector3.down * j, 0.5f, grid[i].possibilities[j]);
                }
            }
        }
    }
    private void DrawGizmosSegment(Vector3 pos, float size, FlipSegment flipSegment) {
        for (int d = 0; d < DirExt.directions.Length; d++) {
            Gizmos.color = flipSegment.GetSocket(d).color;
            Gizmos.DrawLine(pos, pos + (Vector3)(Vector2)DirExt.directions[d] * size);
        }
    }
    private void DrawGizmosSegmentPSockets(Vector3 pos, float size, PossibilityTile cell) {
        for (int d = 0; d < DirExt.directions.Length; d++) {
            float segSize = size / cell.possibleSockets[d].Count;
            int i = 0;
            foreach (Socket s in cell.possibleSockets[d]) {
                float t1 = (float)i / cell.possibleSockets[d].Count;
                Vector3 p1 = pos + Vector3.Lerp(Vector3.zero, (Vector2)DirExt.directions[d], t1);
                Vector3 p2 = p1 + (Vector3)(Vector2)DirExt.directions[d] * segSize;
                Gizmos.color = s.color;
                Gizmos.DrawLine(p1, p2);
                i++;
            }
        }
    }
    #endregion
}
