/// Egy node a pathfindinghoz, amely a Whiskey es Arany megtalalasahoz van.
/// Tartalmazza a poziciot, a  parentnode-ot, es a costokat:
/// - G: koltseg a startol, 
/// - H: heuristic cost, 
/// - F: a osszeguk.
public class PathNode
{
    // Nodes pozik
    public (int, int) Position { get; }
    public PathNode Parent { get; }
    // cost a kezdo nodetol
    public int G { get; }
    // Heuristic cost (manhatten tavolsaggal)
    public int H { get; }
    // teljes cost (G + H)
    public int F => G + H;

    public PathNode((int, int) position, PathNode parent, int g, int h)
    {
        Position = position;
        Parent = parent;
        G = g;
        H = h;
    }
}
