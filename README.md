# ScratchCode
Starting a code library for myself.  Usualy C# but not necessarily Unity Related.

SimpleWASDController.cs is an update to WASController.cs - I removed code and namespaces that weren't relevant.
  
  SimpleWASDController uses NavMeshAgent.move to move an avatar via the WASD keys. It also manages a simple movement animation via the AnimatorController.  The animator float *speed* is a percentage (0-1.0 inclusive) that controls an animator controller blend-tree.  Currently, it takes input from the old Input system, but that should be easy enough to change.
