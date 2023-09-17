shader_type spatial;

uniform sampler2D WaterTexture;
uniform sampler2D DirtTexture;
uniform sampler2D StoneTexture;
uniform float CellSize;

varying float y_coord;

void vertex() {
    y_coord = VERTEX.y;
}

void fragment() {
	if (y_coord < CellSize - 0.01f) 
		ALBEDO = texture(WaterTexture, UV).rgb;
    else if (y_coord > CellSize + 0.01f) 
		ALBEDO = texture(StoneTexture, UV).rgb;
    else 
        ALBEDO = texture(DirtTexture, UV).rgb;
}