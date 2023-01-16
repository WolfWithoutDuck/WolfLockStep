using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.PathFinding;
using Lockstep.Util;
using UnityEngine;

namespace Logic.Managers
{
    public class NavMeshManager
    {
        private static TrianglePointPath path = new TrianglePointPath();
        private static TriangleNavMesh NavMesh;
        public TextAsset navData;
        private static int _mapId = -1;

        public static void DoInit(string data)
        {
            NavMesh = new TriangleNavMesh(data);
        }

        public static List<LVector3> allFindReq = new List<LVector3>();

        public static List<LVector3> FindPath(LVector3 fromPoint, LVector3 toPoint)
        {
            Profiler.BeginSample("FindPath");
            var _ret = NavMesh.FindPath(fromPoint, toPoint, path);
            Profiler.EndSample();
            return _ret;
        }

        private static BspTree _bspTree => NavMesh.bspTree;
    }
}