enemy_turn_length = 3000

player_turn_length = 2000

function enemy_check_tower_ai()
	if tower.Winner == "Alliance" then
		if tower:Count() > 3 then
			if tower:GetOwnerFromIndex(0) == "Opposition" then
				tower:RemoveBottomBall()
				return true
			else
				return false
			end
		elseif tower:Count() == 3 then
			tower:RemoveTopBall()
			return true
		else
			clicked_square:Invoke(tower, e)
			return true
		end
	else
		return false
	end
end

function no_non_neutral_or_empty_tower_enemy_ai()
	math.randomseed( os.time() )
	clicked_square:Invoke(craft_sender_from_point:Invoke(math.random(0, 2), math.random(0, 2)), e)
end

function return_auton_times(stage, team)
	if team == "Alliance" then
		if stage == 1 then
			return 200
		elseif stage == 2 then
			return 3000
		elseif stage == 3 then
			return 5000
		else
			return 15000 - 3000 - 200 - 5000
		end
	else
		if stage == 1 then
			return 1000
		elseif stage == 2 then
			return 500
		else
			return 15000 - 1000 - 500
		end
	end
end

function player_auton(stage)
	tower_1_1:SetOwner("Alliance")
	tower_1_1:ClickableAlliance(true)
	if stage == 1 then
		clicked_square:Invoke(tower_1_0, e)
	elseif stage == 2 then
		clicked_square:Invoke(tower_1_1, e)
	elseif stage == 3 then
		clicked_square:Invoke(tower_1_2, e)
	end
	update_field:Invoke()
end

function enemy_auton(stage)
	tower_1_1:SetOwner("Opposition")
	if stage == 1 then
		clicked_square:Invoke(tower_1_1, e)
	elseif stage == 2 then
		clicked_square:Invoke(tower_2_1, e)
	end
	
	update_field:Invoke()
end