using System.Collections.Generic;

public static class Building
{
    public enum Type { NONE, CASTLE, SAWMILL, QUARRY, WINDMILL, GRAIN };
    public class Cost {
        public readonly int lumber, stone, wheat;
        public Cost(int lumber, int stone, int wheat) {
            this.lumber = lumber;
            this.stone = stone;
            this.wheat = wheat;
        }
    }

    // This dictionary specifies the costs of different buildings 
    public static Dictionary<Type, Cost> Costs = new() {
        { Type.NONE, new Cost(0, 0, 0) },
        { Type.CASTLE, new Cost (0, 0, 0) },
        { Type.SAWMILL, new Cost (5, 4, 0) },
        { Type.QUARRY, new Cost (7, 0, 0) },
        { Type.WINDMILL, new Cost (6, 3, 0) },
        { Type.GRAIN, new Cost (0, 0, 1) },
    };

    // This dictionary indicates what building produces what resource
    public static Dictionary<Type, GameHandler.Resources> Products = new() {
        { Type.SAWMILL, GameHandler.Resources.LUMBER },
        { Type.QUARRY, GameHandler.Resources.STONE },
        { Type.WINDMILL, GameHandler.Resources.WHEAT },
    };

    // These two dictionaries specifies how many resources a building generates
    public static readonly Dictionary<(Type, Type), int> RES_BUILDING_MODIFIERS = new() {
        { (Type.SAWMILL, Type.SAWMILL), -5 },
    };
    public static readonly Dictionary<(Type, HexCell.Type), int> RES_TYPE_MODIFIERS = new() {
        { (Type.SAWMILL, HexCell.Type.FOREST), 1 },
    };
}
