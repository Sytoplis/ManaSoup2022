using UnityEngine;

[System.Serializable]
public class Socket2D
{
    //length 4
    //order: Top, Right, Bottom, Left
    public Socket[] sockets;
    public Socket2D() {
        sockets = new Socket[4];
    }

    public Socket GetSocket(int i) { return sockets[i]; }
    public void SetSocket(int i, Socket s) { sockets[i] = s; }

    public Socket2D Clone() {
        Socket2D s = new Socket2D();
        s.sockets = new Socket[4];
        for (int d = 0; d < 4; d++)
            s.sockets[d] = sockets[4];
        return s;
    }

    public bool Equals(Socket2D other) {
        for (int d = 0; d < 4; d++)
            if (sockets[d] != other.sockets[d])
                return false;
        return true;
    }

    public bool IsCollisionOnly() {
        for (int d = 0; d < 4; d++)
            if (!sockets[d].isCollision)
                return false;
        return true;
    }

    public override string ToString() {
        string str = "{";
        for (int d = 0; d < 4; d++)
            str += $" {(Direction)d}: {sockets[d]} ;";
        return str + "}";
    }
}
