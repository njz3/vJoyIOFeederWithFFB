require("model2")

function Init()

end

function Frame()
	local state = I960_ReadByte(0x202098);
	local state2 = I960_ReadByte(0x20209C);
	local state3 = I960_ReadByte(0x2020AC);

	if state==3 and state2==5 and state3==1 then	
		Model2_SetWideScreen(0)
	else
		Model2_SetWideScreen(1)
	end

end