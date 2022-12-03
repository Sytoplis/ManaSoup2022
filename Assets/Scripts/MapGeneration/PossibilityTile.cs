using System;
using System.Collections.Generic;
using UnityEngine;

public partial class MapGenerator {
    public class PossibilityTile {
        public List<FlipSegment> possibilities;
        public float weightSum;
        public HashSet<Socket>[] possibleSockets;

        public PossibilityTile(in FlipSegment[] segments, float weightSum, in List<Socket>[] possibleSockets) {
            possibilities = new List<FlipSegment>(segments);
            this.weightSum = weightSum;

            //Create all the hash sets
            this.possibleSockets = new HashSet<Socket>[4];
            for (int d = 0; d < 4; d++)
                this.possibleSockets[d] = new HashSet<Socket>(possibleSockets[d]);
        }

        public void SetPossibleSockets(int d, List<Socket> possibleSockets) {
            this.possibleSockets[d] = new HashSet<Socket>(possibleSockets);
        }

        public void RemoveInvalidTiles() {
            //remove invalid tiles
            for (int p = 0; p < possibilities.Count; p++)
                for (int d = 0; d < 4; d++) {
                    if (!possibleSockets[d].Contains(possibilities[p].GetSocket(d))) {//if the socket of the possibility in direction d is NOT part of the possible sockets
                        weightSum -= possibilities[p].GetWeight();//remove the weight from the sum
                        possibilities.RemoveAt(p);//remove possibility hence the sockets dont fit
                        break;
                    }
                }
            RegeneratePossibleSockets();
        }

        public void RegeneratePossibleSockets() {
            for (int d = 0; d < 4; d++) {
                possibleSockets[d].Clear();
                for (int p = 0; p < possibilities.Count; p++) {
                    if (!possibleSockets[d].Contains(possibilities[p].GetSocket(d)))
                        possibleSockets[d].Add(possibilities[p].GetSocket(d));
                }
            }
        }






        public bool SetResult(FlipSegment socket) {
            if (!possibilities.Contains(socket)) return false;//dont collapse on an impossible state

            ForceResult(socket);
            return true;
        }
        public void ForceResult(FlipSegment socket) {
            possibilities.Clear();
            possibilities.Add(socket);

            ReloadPossibleSockets();
        }


        //Return true if the box was fully collapsed
        public int OnlyAllow(HashSet<Socket> sockets, Direction dir) {
            int d = (int)dir;

            //---------- REMOVE SOCKETS ----------------
            //remove every socket in this direction that doesnt fit with one of the given "sockets"
            possibleSockets[d].RemoveWhere(s => !s.MatchesToOneOf(in sockets));


            //---------- REMOVE TILES ------------------
            int removeCount = 0;
            for (int p = possibilities.Count - 1; p >= 0; p--) {
                if (!possibleSockets[d].Contains(possibilities[p].GetSocket(d))) {//if the possible sockets dont have this socket -> remove possibility
                    weightSum -= possibilities[p].GetWeight();//update weight to make still correct calculations
                    possibilities.RemoveAt(p);
                    removeCount++;
                }
            }

            //update sockets from possible changes of the tiles
            ReloadPossibleSockets();

            if (possibilities.Count == 0) {
                string exception = "ALL possibilities for this tile have been removed";
                Debug.LogWarning(exception);
                throw new Exception(exception);
            }
            return removeCount;
        }


        public void ReloadPossibleSockets() {
            for (int d = 0; d < 4; d++) {
                possibleSockets[d].Clear();
                for (int p = 0; p < possibilities.Count; p++) {
                    if (!possibleSockets[d].Contains(possibilities[p].GetSocket(d)))
                        possibleSockets[d].Add(possibilities[p].GetSocket(d));
                }
            }
        }


        public int GetWeightedRnd() {
            float rnd = UnityEngine.Random.Range(0.0f, weightSum);

            float current = 0;
            for (int i = 0; i < possibilities.Count; i++) {
                current += possibilities[i].GetWeight();
                if (current > rnd)
                    return i;
            }

            Debug.LogError("Error with Weighted Randomness");
            return 0;
        }
    }
}