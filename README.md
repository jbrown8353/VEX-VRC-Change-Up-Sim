# VEX-VRC-Change-Up-Sim
Simulator for the Change Up VRC Game, written in C#.

Enemy behaviour and player and enemy autonomous sections written in the accompanying lua file.

Lua Script Definitions:

enemy_check_tower_ai ->
  This function determines whether a random tower is sutible for placing balls in.
  The function returns true if it could mess with the tower, and false, if it needs to search the rest.
  
no_non_neutral_or_empty_tower_enemy_ai ->
  Controls the enemy behaviour when there are no towers to raid and no towers that are empty.
  In the default lua code, it just places a random ball in a random tower.
  
return_auton_times -> 
  Determines how long it takes to complete a certain stage of the auton. For example, in the default lua code, if the alliance member is in the first stage of it's auton, it takes 200 milliseconds until it moves on to the next action. Simulates the robot moving if you want to look at it like that.
  
player_auton ->
  Complete's the player's auton based on it's current stage.

enemy_auton ->
  Complete's the enemy's auton based on it's current stage.
  
enemy_turn_length ->
  Determines how long until the enemy can play again. Simulates the robot moving.
  
player_turn_length ->
  Determines how long until the player can act again.
  
Syntax:

tower:Winner -> Returns the winner of the tower if there is one.
tower:Count() -> Returns the amount of balls in a tower.
tower:GetOwnerFromIndex(n) -> Returns the nth ball in the tower's owner.
tower:RemoveBottomBall() -> Removes the bottom ball from the tower.
tower:RemoveTopBall() -> Removes the top ball from the tower.
tower_r_c -> Gets the tower at row r and column c.


clicked_square:Invoke(tower, e) -> Puts a ball in a tower.

