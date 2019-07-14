
local Centerpiece;
local Particles = { };

function AsyncLoad()
	Centerpiece = res.QueueTextureLoad("textures/game_bg/centerpiece");
	Particles[1] = res.QueueTextureLoad("textures/game_bg/particle0");
	Particles[2] = res.QueueTextureLoad("textures/game_bg/particle1");
	Particles[3] = res.QueueTextureLoad("textures/game_bg/particle2");
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

	g2d.ImageColor(255, 255, 255, 255);
	g2d.Image(Centerpiece, (width - centerSize) / 2, height * HorizonHeight - centerSize * 0.8, centerSize, centerSize);
end
