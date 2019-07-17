
local Layout;
local LayoutWidth, LayoutHeight;
local LayoutScale;

local ViewportWidth, ViewportHeight;

local function CalculateLayout()
	ViewportWidth, ViewportHeight = g2d.GetViewportSize();
	Layout = ViewportWidth > ViewportHeight and "Landscape" or "Portrait";
	if (Layout == "Landscape") then
		if (ViewportWidth / ViewportHeight > 2) then
			Layout = "Wide-" .. Layout;
			LayoutWidth = 1680;
		else
			LayoutWidth = 1280;
		end
	else
		LayoutWidth = 720;
	end
	LayoutScale = ViewportWidth / LayoutWidth;
	LayoutHeight = ViewportHeight / LayoutScale;
end

local function DoLayoutTransform()
	g2d.Scale(LayoutScale, LayoutScale);
end

function AsyncLoad()
	return true;
end

function AsyuncFinalize()
	return true;
end

function Init()
	CalculateLayout();
end

function Update(delta, total)
	do -- check if layout needs to be refreshed
		local cvw, cvh = g2d.GetViewportSize();
		if (ViewportWidth != cvw and ViewportHeight != cvh) then
			CalculateLayout();
		end
	end

	--
end

local function DrawGameStatus(x, y)
	g2d.SaveTransform();
	g2d.Translate(x, y);

	g2d.SetColor(60, 60, 60, 225);
	g2d.FillRect(0, 0, 300, 120);

	local diffPlateHeight = 20;
	local jacketPadding = 5;
	local jacketSize = 120 - 20 - 3 * jacketPadding - diffPlateHeight;

	g2d.SetColor(255, 255, 255);
	g2d.FillRect(10, 10, jacketSize + 2 * jacketPadding, 120 - 20);

	g2d.SetColor(0, 0, 0); -- TEMP, DRAW JACKET IMAGE
	g2d.FillRect(10 + jacketPadding, 10 + jacketPadding, jacketSize, jacketSize);

	local diffColor = game.meta.DifficultyColor;
	g2d.SetColor(diffColor.x, diffColor.y, diffColor.z);
	g2d.FillRect(10 + jacketPadding, 10 + 2 * jacketPadding + jacketSize, jacketSize, diffPlateHeight);

	g2d.SetColor(0, 0, 0);
	g2d.SetFont(nil, 8);
	g2d.Write(game.meta.DifficultyName .. " " .. game.meta.DifficultyLevel, 10 + 2 * jacketPadding, 10 + 2 * jacketPadding + jacketSize + diffPlateHeight / 2);

	g2d.RestoreTransform();
end

function Draw()
	g2d.SaveTransform();
	DoLayoutTransform();

	if (Layout == "Landscape") then
		DrawGameStatus(10, 10);
	
		-- Score
		g2d.SetColor(60, 60, 60, 225);
		g2d.FillRect(LayoutWidth - 10 - 300, 10, 300, 120);
	
		-- Gauge
		g2d.SetColor(60, 60, 60, 225);
		g2d.FillRect(LayoutWidth * 3 / 4 - 35, LayoutHeight / 2 - 200, 70, 400);
		
		-- Chart Info
		g2d.SetColor(60, 60, 60, 225);
		g2d.FillRect(10, LayoutHeight - 10 - 160, 300, 160);
	elseif (Layout == "Portrait") then
	else -- if (Layout == "Wide-Landscape") then
	end

	g2d.RestoreTransform();
end
