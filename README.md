# Self-written 3D Data Reconstruction by Marching Tetrahedra
A side project that works on reconstructing 3D meshes from spatial data. Hopefully it can be used in many future works.

<img width="1037" height="570" alt="1" src="https://github.com/user-attachments/assets/3dac2ba3-83cf-4aa6-811d-8f91ac9ce9e6" />
<img width="1012" height="520" alt="2" src="https://github.com/user-attachments/assets/dea09732-56e2-4aee-b2e4-122521305aa6" />

## Features
- GPU and CPU mode. GPU is obliviously faster but will have some weird bugs when the grid size is really small, also the size of the grid cannot be too big (I suggest keep it under 50). CPU will call on a different thread so I don't recommend updating it per-frame.
- Easy to use interface.

## Tutorial
- Create a new game object, attach script ./Script/CanvasGrid to it.
- In the inspector you can configure the grid, such as whole canvas size, the number of segment (represented as "grid"), and the surface value for the mesh.
- You can also switch between GPU and CPU.
- To actually feed the point cloud data to the grid, refer the CanvasGrid component in your script (here I refer it as grid) and call grid.SetGrid(x,y,z,val) to set a single value on a grid of a certain coordinate. Then call grid.UpdateMesh() to reconstruct the mesh.

- # Future Works
- - Improve on the interpolation - the current one doesn't look anywhere near good.
  - Better point cloud representation.
  - Reorganize the code to make it more readable.
  - Multithread in CPU mode.
