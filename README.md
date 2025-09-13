# Self-written 3D Data Reconstruction by Marching Tetrahedra
A side project that works on reconstructing 3D meshes from spatial data. Hopefully it can be used in many future works.


<img width="913" height="394" alt="1" src="https://github.com/user-attachments/assets/fdbf249e-8582-42d2-a83f-af2bc0e751ce" />
<img width="729" height="395" alt="2" src="https://github.com/user-attachments/assets/72051a34-0927-4f80-8ae3-32a98810e9e4" />


## Features
- GPU and CPU mode. GPU is obliviously faster but will have some weird bugs when the grid size is really small.
- Easy to use interface.

## Tutorial
- Create a new game object, attach script ./Script/CanvasGrid to it.
- In the inspector you can configure the grid, such as whole canvas size, the number of segment (represented as "grid"), and the max value for the point cloud data.
- You can also switch between GPU and CPU.
- To actually feed the point cloud data to the grid, refer the CanvasGrid component in your script (here I refer it as grid) and call grid.SetGrid(x,y,z,val) to set a single value on a grid of a certain coordinate. Then call grid.UpdateMesh() to reconstruct the mesh.

- # Future Works
- - Improve on the interpolation - the current one doesn't look anywhere near good.
  - Better point cloud representation.
  - Reorganize the code to make it more readable.
  - Multithread in CPU mode.
