
local BackTexture;

function AsyncLoad()
	BackTexture = res.QueueTextureLoad("textures/game_bg/background");
end

function AsyncFinalize()
end

function Init()
end

function Update(delta, total)
end

function Draw()
	g2d.Image(BackTexture, 0, 0, 1280, 720);
end
