using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CubeData
{
    // Represents the whole rubik's cube
    public class Cube
    {
        public static readonly Cubie[,,] DefaultCube = {
        {
            {new Cubie(-57,-21,-47), new Cubie(  0,-22,-46), new Cubie( 65,-23,-45)},
            {new Cubie(-58,-28,  0), new Cubie(  0,-20,  0), new Cubie( 64,-24,  0)},
            {new Cubie(-51,-27, 31), new Cubie(  0,-26, 32), new Cubie( 63,-25, 33)}
        },
        {
            {new Cubie(-56,  0,-48), new Cubie(  0,  0,-40), new Cubie( 66,  0,-44)},
            {new Cubie(-50,  0,  0), new Cubie(  0,  0,  0), new Cubie( 60,  0,  0)},
            {new Cubie(-52,  0, 38), new Cubie(  0,  0, 30), new Cubie( 62,  0, 34)}
        },
        {
            {new Cubie(-55, 17,-41), new Cubie(  0, 16,-42), new Cubie( 67, 15,-43)},
            {new Cubie(-54, 18,  0), new Cubie(  0, 10,  0), new Cubie( 68, 14,  0)},
            {new Cubie(-53, 11, 37), new Cubie(  0, 12, 36), new Cubie( 61, 13, 35)}
        }
    };

        public static readonly Cubie[,,] SingleCubie = { { { new Cubie(-57, -21, -47) } } };

        public Cubie[,,] Cubies = DefaultCube;

        #region Layer Rotation
        public void RotYp90d(int i_layer)
        {
            Cubie[,,] oldCubies = Cubies.Clone() as Cubie[,,];
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
            Cubie[,,] oldCubies = Cubies.Clone() as Cubie[,,];
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
            Cubie[,,] oldCubies = Cubies.Clone() as Cubie[,,];
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
            Cubie[,,] oldCubies = Cubies.Clone() as Cubie[,,];
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
            Cubie[,,] oldCubies = Cubies.Clone() as Cubie[,,];
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
            Cubie[,,] oldCubies = Cubies.Clone() as Cubie[,,];
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

}
