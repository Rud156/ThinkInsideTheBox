using UnityEngine;
using CubeData;
namespace WorldCube
{
    [System.Serializable]
    public class CubeLayerMaskV2
    {
        [Header("Layer Sides")] public int x;
        public int y;
        public int z;

        public int X => x;
        public int Y => y;
        public int Z => z;

        public static CubeLayerMaskV2 Zero => new CubeLayerMaskV2();

        public static CubeLayerMaskV2 Right => new CubeLayerMaskV2(1, 0, 0);

        public static CubeLayerMaskV2 Left => new CubeLayerMaskV2(-1, 0, 0);

        public static CubeLayerMaskV2 Up => new CubeLayerMaskV2(0, 1, 0);

        public static CubeLayerMaskV2 Down => new CubeLayerMaskV2(0, -1, 0);

        public static CubeLayerMaskV2 Forward => new CubeLayerMaskV2(0, 0, 1);

        public static CubeLayerMaskV2 Back => new CubeLayerMaskV2(0, 0, -1);

        public CubeLayerMaskV2()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public CubeLayerMaskV2(int i_x, int i_y, int i_z)
        {
            x = i_x;
            y = i_y;
            z = i_z;
        }

        public CubeLayerMaskV2(CubeLayerMask i_other)
        {
            x = i_other.x;
            y = i_other.y;
            z = i_other.z;
        }

        public bool IsValid()
        {
            if (x * y != 0 || x * z != 0 || y * z != 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsCombinedNotZero(CubeLayerMaskV2 i_lhs, CubeLayerMaskV2 i_rhs)
        {
            if (!i_lhs.IsValid() || !i_rhs.IsValid())
            {
                return false;
            }

            if (i_lhs.x * i_rhs.x != 0 || i_lhs.y * i_rhs.y != 0 || i_lhs.z * i_rhs.z != 0)
            {
                return true;
            }

            return false;
        }

        public static bool operator ==(CubeLayerMaskV2 i_lhs, CubeLayerMaskV2 i_rhs)
        {
            if (i_lhs is null || i_rhs is null)
            {
                return false;
            }

            return i_lhs.Equals(i_rhs);
        }

        public static bool operator !=(CubeLayerMaskV2 i_lhs, CubeLayerMaskV2 i_rhs)
        {
            return !(i_lhs == i_rhs);
        }

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            CubeLayerMaskV2 mask = (CubeLayerMaskV2) other;
            return mask.x == x && mask.y == y && mask.z == z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
    }
}