# ScratchCode
Starting a code library for myself.  Usualy C# but not necessarily Unity Related.

PlayerMotor.cs is an interesting take (to me anyway) of NavMeshAgent based character movement.  While the left mouse button is held down, the character turns to face the mouse cursor and starts moving straight towards it.  The further away the mouse cursor is, the faster the character moves.  But, there is no Pathing involved - which means no obstacle avoidance is happening.  It makes use of a simplistic animation controller: ![image](https://user-images.githubusercontent.com/5873443/165970406-0b31f435-b1cd-43f5-a926-e79bcca0ff62.png)

SimpleWASDController.cs is an update to WASController.cs - I removed code and namespaces that weren't relevant.
  
  SimpleWASDController uses NavMeshAgent.move to move an avatar via the WASD keys. It also manages a simple movement animation via the AnimatorController.  The animator float *speed* is a percentage (0-1.0 inclusive) that controls an animator controller blend-tree.  Currently, it takes input from the old Input system, but that should be easy enough to change.

BitMapHelper.cs came from https://github.com/mesta1/AForge-examples
