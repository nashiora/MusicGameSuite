
local _meta_string = getmetatable('');

function _meta_string.__index(str, i)
	return string.sub(str, i, i);
end

function typeof(value)
	if (type(value) == "table") then
		local meta = getmetatable(value);
		if (meta.__type) then
			return meta.__type;
		end
	end
	return type(value);
end
