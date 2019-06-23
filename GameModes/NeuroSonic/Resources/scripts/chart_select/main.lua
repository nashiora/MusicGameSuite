
local timer = 0

function Update(delta)
    timer = timer + delta;
end

function Draw()
    gfx.SetColor(255, 0, 255);
    gfx.Translate(10, 10);
    gfx.FillRect(0, 0, 100, 100);
end
