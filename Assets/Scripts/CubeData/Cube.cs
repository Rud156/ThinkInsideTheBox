using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    // Represents the whole rubik's cube
    public class Cube
    {
        public static readonly Cubie[,,] DefaultCube = new Cubie[,,] {
        // Layer down   y = -1
        {
            // || Layer left,x = -1 ||   Layer mid, x = 0  ||  Layer right, x = 1 ||
            {new Cubie(-57,-21,-47), new Cubie(  0,-22,-46), new Cubie( 65,-23,-45)},// Layer front,    z = -1
            {new Cubie(-58,-28,  0), new Cubie(  0,-20,  0), new Cubie( 64,-24,  0)},// Layer mid,      z = 0
            {new Cubie(-51,-27, 31), new Cubie(  0,-26, 32), new Cubie( 63,-25, 33)} // Layer back,     z = 1
        },
        
        // Layer mid,   y = 0
        {
            // || Layer left,x = -1 ||   Layer mid, x = 0  ||  Layer right, x = 1 ||
            {new Cubie(-56,  0,-48), new Cubie(  0,  0,-40), new Cubie( 66,  0,-44)},// Layer front,    z = -1
            {new Cubie(-50,  0,  0), new Cubie(  0,  0,  0), new Cubie( 60,  0,  0)},// Layer mid,      z = 0
            {new Cubie(-52,  0, 38), new Cubie(  0,  0, 30), new Cubie( 62,  0, 34)} // Layer back,     z = 1
        },
        // Layer up,    y = 1
        {
            // || Layer left,x = -1 ||   Layer mid, x = 0  ||  Layer right, x = 1 ||
            {new Cubie(-55, 17,-41), new Cubie(  0, 16,-42), new Cubie( 67, 15,-43)},// Layer front,    z = -1
            {new Cubie(-54, 18,  0), new Cubie(  0, 10,  0), new Cubie( 68, 14,  0)},// Layer mid,      z = 0
            {new Cubie(-53, 11, 37), new Cubie(  0, 12, 36), new Cubie( 61, 13, 35)} // Layer back,     z = 1
        }
    };

        public static readonly Cubie[,,] SingleCubie = { { { new Cubie(-57, -21, -47) } } };

        public static Cubie[,,] DeepCopyCubies(Cubie[,,] i_cubies)
        {
            Cubie[,,] cubies = (Cubie[,,])DefaultCube.Clone();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        cubies[i, j, k] = new Cubie(i_cubies[i, j, k]);
                    }
                }
            }
            return cubies;
        }

        public bool HasFinished()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (Cubies[i,j,k] != DefaultCube[i,j,k])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public Cubie[,,] Cubies = DeepCopyCubies(DefaultCube);

        #region Layer Rotation
        public void RotYp90d(int i_layer)
        {
            Cubie[,,] oldCubies = (Cubie[,,])Cubies.Clone();
            for (int i = 0; i < 3; i++)
            {
                if (i != i_layer)
                    continue;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Cubies[i, j, k] = oldCubies[i, k, 2 - j];
                        Cubies[i, j, k].RotYp90d();
                    }
                }
            }
        }

        public void RotYn90d(int i_layer)
        {
            Cubie[,,] oldCubies = (Cubie[,,])Cubies.Clone();
            for (int i = 0; i < 3; i++)
            {
                if (i != i_layer)
                    continue;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Cubies[i, j, k] = oldCubies[i, 2 - k, j];
                        Cubies[i, j, k].RotYn90d();
                    }
                }
            }
        }
        public void RotZp90d(int i_layer)
        {
            Cubie[,,] oldCubies = (Cubie[,,])Cubies.Clone();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j != i_layer)
                        continue;
                    for (int k = 0; k < 3; k++)
                    {
                        Cubies[i, j, k] = oldCubies[2 - k, j, i];
                        Cubies[i, j, k].RotZp90d();
                    }
                }
            }
        }
        public void RotZn90d(int i_layer)
        {
            Cubie[,,] oldCubies = (Cubie[,,])Cubies.Clone();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j != i_layer)
                        continue;
                    for (int k = 0; k < 3; k++)
                    {
                        Cubies[i, j, k] = oldCubies[k, j, 2 - i];
                        Cubies[i, j, k].RotZn90d();
                    }
                }
            }
        }
        public void RotXp90d(int i_layer)
        {
            Cubie[,,] oldCubies = (Cubie[,,])Cubies.Clone();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (k != i_layer)
                            continue;
                        Cubies[i, j, k] = oldCubies[j, 2 - i, k];
                        Cubies[i, j, k].RotXp90d();
                    }
                }
            }
        }
        public void RotXn90d(int i_layer)
        {
            Cubie[,,] oldCubies = (Cubie[,,])Cubies.Clone();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (k != i_layer)
                            continue;
                        Cubies[i, j, k] = oldCubies[2 - j, i, k];
                        Cubies[i, j, k].RotXn90d();
                    }
                }
            }
        }
        #endregion
    }

    [Serializable]
    // Represents the small individual cube
    public class Cubie
    {
        [Header("Indexes")]
        public int x;
        public int y;
        public int z;

        public Cubie()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public Cubie(int i_x, int i_y, int i_z)
        {
            this.x = i_x;
            this.y = i_y;
            this.z = i_z;
        }

        public Cubie(Cubie i_cubie)
        {
            this.x = i_cubie.x;
            this.y = i_cubie.y;
            this.z = i_cubie.z;
        }

        public static bool operator ==(Cubie i_lhs, Cubie i_rhs)
        {
            if (i_lhs is null)
                return i_rhs is null;
            return i_lhs.Equals(i_rhs);
        }

        public static bool operator !=(Cubie i_lhs, Cubie i_rhs)
        {
            return !(i_lhs == i_rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var cubie = (Cubie)obj;
            return this.x == cubie.x && this.y == cubie.y && this.z == cubie.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        #region Cubie Rotation
        public void RotXp90d()
        {
            Cubie oldCubie = new Cubie(this);
            this.x = oldCubie.x;
            this.y = -oldCubie.z;
            this.z = oldCubie.y;
        }

        public void RotXn90d()
        {
            Cubie oldCubie = new Cubie(this);
            this.x = oldCubie.x;
            this.y = oldCubie.z;
            this.z = -oldCubie.y;
        }

        public void RotYp90d()
        {
            Cubie oldCubie = new Cubie(this);
            this.x = oldCubie.z;
            this.y = oldCubie.y;
            this.z = -oldCubie.x;
        }

        public void RotYn90d()
        {
            Cubie oldCubie = new Cubie(this);
            this.x = -oldCubie.z;
            this.y = oldCubie.y;
            this.z = oldCubie.x;
        }

        public void RotZp90d()
        {
            Cubie oldCubie = new Cubie(this);
            this.x = -oldCubie.y;
            this.y = oldCubie.x;
            this.z = oldCubie.z;
        }

        public void RotZn90d()
        {
            Cubie oldCubie = new Cubie(this);
            this.x = oldCubie.y;
            this.y = -oldCubie.x;
            this.z = oldCubie.z;
        }
        #endregion
    }

    [Serializable]
    public class CubeLayerMask
    {
        public int x = 0; // 0 means no rotation on this layer
        public int y = 0; // 0 means no rotation on this layer
        public int z = 0; // 0 means no rotation on this layer

        public static CubeLayerMask right => new CubeLayerMask(1, 0, 0);
        public static CubeLayerMask left => new CubeLayerMask(-1, 0, 0);
        public static CubeLayerMask up => new CubeLayerMask(0, 1, 0);
        public static CubeLayerMask down => new CubeLayerMask(0, -1, 0);
        public static CubeLayerMask forward => new CubeLayerMask(0, 0, 1);
        public static CubeLayerMask back => new CubeLayerMask(0, 0, -1);

        public static CubeLayerMask Zero
        {
            get
            {
                return new CubeLayerMask(0, 0, 0); 
            }
        }

        private CubeLayerMask() { } // Prevent default constructor

        public CubeLayerMask(int i_x, int i_y, int i_z)
        {
            if (!CubeLayerMask.IsValid(i_x, i_y, i_z))
                throw new Exception("Invalid CubeLayerMask");
            this.x = i_x;
            this.y = i_y;
            this.z = i_z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(this.x, this.y, this.z);
        }

        public static bool IsValid(int i_x, int i_y, int i_z)
        {
            if (i_x * i_y != 0 || i_x * i_z != 0 || i_y * i_z != 0)
            {
                return false;
            }
            return true;
        }

        public bool IsValid()
        {
            if (x * y != 0 || x * z != 0 || y * z != 0)
                return false;
            return true;
        }

        public static bool operator ==(CubeLayerMask i_lhs, CubeLayerMask i_rhs)
        {
            if (i_lhs is null)
                return i_rhs is null;
            return i_lhs.Equals(i_rhs);
        }

        public static bool operator !=(CubeLayerMask i_lhs, CubeLayerMask i_rhs)
        {
            return !(i_lhs == i_rhs);
        }

        public static CubeLayerMask operator -(CubeLayerMask i_rhs) => new CubeLayerMask(-i_rhs.x, -i_rhs.y, -i_rhs.z);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var cubie = (CubeLayerMask)obj;
            return this.x == cubie.x && this.y == cubie.y && this.z == cubie.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public static bool operator ^(CubeLayerMask i_lhs, CubeLayerMask i_rhs)
        {
            if (!i_lhs.IsValid() || !i_rhs.IsValid())
                return false;
            if (i_lhs.x * i_rhs.x != 0 || i_lhs.y * i_rhs.y != 0 || i_lhs.z * i_rhs.z != 0)
                return true;
            return false;
        }
    }
}
