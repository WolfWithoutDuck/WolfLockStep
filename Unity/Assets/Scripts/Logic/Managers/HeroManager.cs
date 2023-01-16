using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic
{
    public static GameObject CureateEntity(BaseEntity entity, int prefabId, LVector3 position,
        GameObject prefab, object config)
    {
        var obj = GameObject.Instantiate(prefab, position.ToVector3(), Quaternion.identity);
        entity.engineTransform = obj.transform;
        entity.transform.Pos3 = position;
        config.
        
    }


    public class HeroManager
    {
        public static void InstantiateEntity(Player allPlayer, int playerInfoPrefabId, LVector3 playerInfoInitPos)
        {
        }
    }
}