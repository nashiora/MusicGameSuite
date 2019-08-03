
local _vec2 = vec2;
local _vec3 = vec3;
local _vec4 = vec4;

local _meta_vec2, _meta_vec3, _meta_vec4;

local componentNames = { "x", "y", "z", "w" };

local function MakeVec2(v) return setmetatable({ _v = v }, _meta_vec2); end
local function MakeVec3(v) return setmetatable({ _v = v }, _meta_vec3); end
local function MakeVec4(v) return setmetatable({ _v = v }, _meta_vec4); end

local function GetVecOf(values)
	if (#values == 2) then
		return MakeVec2(_vec2(table.unpack(values)));
	elseif (#values == 3) then
		return MakeVec3(_vec3(table.unpack(values)));
	elseif (#values == 4) then
		return MakeVec4(_vec4(table.unpack(values)));
	end
	return nil;
end

local function GenerateVecMeta(count, ctor)
	local typeName = "vec" .. count;

	local function CreateStrictOperator(name, op)
		return function(self, other)
			local otherType = typeof(other);
			if (otherType == typeName) then
				local result = op(rawget(self, "_v"), rawget(other, "_v"));
				return ctor(result);
			end
			error("Type mismatch: attempt to " .. name .. " " .. otherType .. " to " .. typeName);
		end;
	end
	
	local function CreateScalarOperator(name, op)
		return function(self, other)
			local otherType = typeof(other);
			if (otherType == typeName) then
				local result = op(rawget(self, "_v"), rawget(other, "_v"));
				return ctor(result);
			elseif (otherType == "number") then
				local result = op(rawget(self, "_v"), other);
				return ctor(result);
			end
			error("Type mismatch: attempt to " .. name .. " " .. otherType .. " to " .. typeName);
		end;
	end

	return {
		__type = typeName,
		--__functions = { },
		
		__unm = function(self) return ctor(-rawget(self, "_v")); end,
		__add = CreateStrictOperator("add", |l, r| l + r),
		__sub = CreateStrictOperator("subtract", |l, r| l - r),
		__mul = CreateScalarOperator("multiply", |l, r| l * r),
		__div = CreateScalarOperator("divide", |l, r| l / r),

		__index = function(self, index)
			local v = rawget(self, "_v");
			local indexType = typeof(index);

			if (indexType == "string") then
				local len = string.len(index);

				if (len == 1) then
					return v[index];
				elseif (len >= 2 and len <= 4) then
					if (len > count) then
						error("Attempt to index " .. count .. " component vector with " .. len .. " indices");
					end

					local swizzle = { };
					for i = 1, len do
						table.insert(swizzle, v[index[i]]);
					end
					return GetVecOf(swizzle);
				end
			elseif (indexType == "number") then
				if (len > count) then
					error("Attempt to index " .. count .. " component vector with " .. len .. " indices");
				elseif (math.floor(index) ~= index) then
					error("Attempt to index vector with fraction value");
				end

				return v[componentNames[index]];
			end

			error("Attempt to index field " .. index .. " of " .. typeName);
		end,
	
		__newindex = function(self, index, value)
			local v = rawget(self, "_v");
			local indexType = typeof(index);

			if (indexType == "string") then
				local len = string.len(index);

				if (len == 1) then
					v[index] = value;
				elseif (len >= 2 and len <= 4) then
					local valueType = typeof(value);
					if (len > count) then
						error("Attempt to index " .. count .. " component vector with " .. len .. " indices");
					elseif (valueType ~= "vec" .. len) then
						error("Attempt to assign " .. valueType .. " to " .. len .. " component swizzle")
					end

					for i = 1, len do
						v[index[i]] = rawget(value, "_v")[componentNames[i]];
					end
				end
			elseif (indexType == "number") then
				if (len > count) then
					error("Attempt to index " .. count .. " component vector with " .. len .. " indices");
				elseif (math.floor(index) ~= index) then
					error("Attempt to index vector with fraction value");
				end

				v[componentNames[index]] = value;
			else
				error("Attempt to index field " .. index .. " of " .. typeName);
			end
		end,

		__tostring = function(self) return tostring(rawget(self, "_v")); end,
	};
end

_meta_vec2 = GenerateVecMeta(2, MakeVec2);
_meta_vec3 = GenerateVecMeta(3, MakeVec3);
_meta_vec4 = GenerateVecMeta(4, MakeVec4);

function vec2(x, y)
	if (not x and not y) then
		x, y = 0, 0;
	elseif (not y) then
		y = x;
	end
	-- note, we still don't know if these are numbers
	return MakeVec2(_vec2(x, y));
end

return MakeVec2, MakeVec3, MakeVec4;
