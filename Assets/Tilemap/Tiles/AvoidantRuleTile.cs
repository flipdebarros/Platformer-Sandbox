using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class AvoidantRuleTile : RuleTile<AvoidantRuleTile.Neighbor> {

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int NotNull = 1;
        public const int Null = 2;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
    
        if (tile is RuleOverrideTile)
            tile = (tile as RuleOverrideTile).m_InstanceTile;
    
        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }

        return true;
    }
}