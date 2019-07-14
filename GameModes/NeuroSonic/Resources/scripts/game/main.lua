
local Centerpiece;

function AsyncLoad()
	Centerpiece = res.QueueTextureLoad("textures/game_bg/centerpiece");
end

function AsyncFinalize()
end

function Init()
end

function Update(delta, total)
end

function Draw()
	local width, height = window.GetClientSize();
	local centerSize = width * 0.6;
	g2d.Image(Centerpiece, (width - centerSize) / 2, height * HorizonHeight - centerSize * 0.8, centerSize, centerSize);
end
