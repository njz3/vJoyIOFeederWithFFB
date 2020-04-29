require("model2");	-- Import model2 machine globals

function Init()
	Patch_SpecialInputs();
	Patch_LampOutputs();
end

function Frame()
	local gameState = I960_ReadByte(0x5010A4)

	if   gameState==0x16	-- Ingame
	  or gameState==0x03	-- Attract ini
	  or gameState==0x04	-- Attract Higscore ini
	  or gameState==0x05	-- Attract Highscore
	  or gameState==0x06	-- Attract VR Ini
	  or gameState==0x07	-- Attract VR
	then
	 	Model2_SetStretchBLow(1)	-- Stretch the bg tilemap (sky & clouds) when widescreen
		Model2_SetWideScreen(1)
	else					-- No widescreen on the rest of the screens
	 	Model2_SetStretchBLow(0)
		Model2_SetWideScreen(0)
	end

end

function Patch_SpecialInputs()
	-- first, disable old read
	Romset_PatchDWord(0, 0x1E504, 0x5CA01E00);	-- MOV g4,0x00 (NOOP?)
	
	-- now jump to our patched read
	Romset_PatchDWord(0, 0x1E508, 0x090219F8);	-- CALL 0x0003FF00

	-- read io port
	Romset_PatchDWord(0, 0x3FF00, 0x80A03000);
	Romset_PatchDWord(0, 0x3FF04, 0x01C00012);	-- LDOB g4,0x01C00012

	-- read patched mask
	Romset_PatchDWord(0, 0x3FF08, 0x80B83000);
	Romset_PatchDWord(0, 0x3FF0C, 0x00500820);	-- LDOB g7,0x00500820

	-- and em
	Romset_PatchDWord(0, 0x3FF10, 0x58A50097);	-- AND g4,g4,g7

	-- restore old mask
	Romset_PatchDWord(0, 0x3FF14, 0x8CB800FF);	-- LDA g7,0xff

	-- return
	Romset_PatchDWord(0, 0x3FF18, 0x0A000000);  -- RET
end

function Patch_LampOutputs()
	-- reroute 0x01C0001E to 0x00500824
	for offset = 0x00000000, 0x0003FFFF, 4 do
		if Romset_ReadDWord(0, offset) == 0x01C0001E then
			Romset_PatchDWord(0, offset, 0x00500824);
			local opcode = offset - 1;
			if Romset_ReadByte(0, opcode) == 0x80 then
				Romset_PatchByte(0, opcode, 0x90)	-- replace LDOB with LD
			end
			if Romset_ReadByte(0, opcode) == 0x82 then
				Romset_PatchByte(0, opcode, 0x92)	-- replace STOB with ST
			end
		end
	end
end

function PostDraw()
	if I960_ReadByte(RAMBASE + 0x00000820) == 0x00 then
		I960_WriteByte(RAMBASE + 0x00000820, 0xFF);
	end
	-- for debug only
	Video_DrawText(0,0,HEX8(I960_ReadByte(0x00500824)),0xFFFFFF);
end

--Some sample code follows to show how to draw strings and setup options/cheats
--
--function PostDraw()
--	Video_DrawText(20,10,HEX32(I960_GetRamPtr(RAMBASE)),0xFFFFFF);
--	Video_DrawText(20,10,HEX32(I960_ReadWord(RAMBASE+0x10D0)),0xFFFFFF);
--	Video_DrawText(20,20,HEX32(RAMBASE),0xFFFFFF);
--	Video_DrawText(20,30,Options.cheat1.value,0xFFFFFF);
--	Video_DrawText(20,40,Input_IsKeyPressed(0x1E),0xFFFFFF);
--end
--
--function cheat1func(value)
--	
--end
--
--function cheat1change(value)
--
--end
--
--

function timecheatfunc(value)
	I960_WriteWord(RAMBASE+0x10D0,50*64);	--50 seconds always
end

Options =
{
--	cheat1={name="Cheat 1",values={"Off","On"},runfunc=cheat1func,changefunc=cheat1change},
	timecheat={name="Infinite Time",values={"Off","On"},runfunc=timecheatfunc}
}
