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
    // The player should be able to build one lumbermill and one quarry before building anything else
    // Start resources are 15 lumber, 7 stone, 2 wheat
    public static Dictionary<Type, Cost> Costs = new() {
        { Type.NONE, new Cost(0, 0, 0) },
        { Type.CASTLE, new Cost (0, 0, 0) },
        { Type.SAWMILL, new Cost (5, 7, 0) },
        { Type.QUARRY, new Cost (10, 0, 0) },
        { Type.WINDMILL, new Cost (6, 8, 0) },
        { Type.GRAIN, new Cost (0, 0, 1) },
    };

    // This dictionary indicates what building produces what resource
    public static Dictionary<Type, GameHandler.Resources> Products = new() {
        { Type.SAWMILL, GameHandler.Resources.LUMBER },
        { Type.QUARRY, GameHandler.Resources.STONE },
        { Type.WINDMILL, GameHandler.Resources.WHEAT },
    };

    // This dictionary contains the display names of the building types
    public static Dictionary<Type, string> DisplayNames = new() {
        { Type.NONE, "Placeholder" },
        { Type.SAWMILL, "Lumbermill" },
        { Type.QUARRY, "Quarry" },
        { Type.WINDMILL, "Windmill" },
        { Type.GRAIN, "Grain Field" },
    };

    // These two dictionaries specifies how many resources a building generates
    public static readonly Dictionary<(Type, Type), int> RES_BUILDING_MODIFIERS = new() {
        { (Type.SAWMILL, Type.SAWMILL), -5 },
        { (Type.WINDMILL, Type.GRAIN), 1 },
        { (Type.WINDMILL, Type.WINDMILL), -5 },
        { (Type.QUARRY, Type.SAWMILL), 2 }, // the lumbermill directly supplies the quarry with wood for tools and support beams
        { (Type.QUARRY, Type.QUARRY), -5 },
    };
    public static readonly Dictionary<(Type, HexCell.Type), int> RES_TYPE_MODIFIERS = new() {
        { (Type.SAWMILL, HexCell.Type.FOREST), 1 },
        { (Type.QUARRY, HexCell.Type.MOUNTAINS), 1 },
    };
}
