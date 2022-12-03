using UnityEngine;

public class FlipSegment
{
    public MapSegment segment;
    public bool flip = false;//if we flip the map segment or not
    public float weightMultiplier = 1.0f;
    public float GetWeight() { return weightMultiplier * segment.weight; }

    public FlipSegment(MapSegment segment, bool flip, float weightMul) {
        this.segment = segment;
        this.flip = flip;
        this.weightMultiplier = weightMul;
    }
    public FlipSegment(MapSegment segment, bool flip) {
        this.segment = segment;
        this.flip = flip;
        this.weightMultiplier = 1.0f;
    }

    public Socket GetSocket(int d) {
        return GetSocket((Direction)d);
    }
    public Socket GetSocket(Direction dir) {
        return segment.socket.GetSocket((int)DirExt.Flip(dir, flip));
    }

    public bool SocketEquals(Socket2D other) {
        for (int d = 0; d < 4; d++)
            if (GetSocket(d) != other.sockets[d])
                return false;
        return true;
    }
    public bool SocketFits(Socket2D incoming) {
        for (int d = 0; d < 4; d++)
            if (!GetSocket(d).Matches(incoming.sockets[d]))
                return false;
        return true;
    }

    public Socket2D GetFlippedSocket2D() {//returns the socket2D that applies to the current flip segment
        Socket2D socket = new Socket2D();
        for (int d = 0; d < 4; d++)
            socket.sockets[d] = GetSocket(d);
        return socket;
    }


    public bool IsEmpty() { return segment.socket.IsCollisionOnly(); }
}
