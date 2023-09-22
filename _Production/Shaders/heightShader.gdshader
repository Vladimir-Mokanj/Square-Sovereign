shader_type spatial;

uniform sampler2D WaterTexture;
uniform sampler2D DirtTexture;
uniform sampler2D StoneTexture;

varying float y_coord;

void vertex() {
    y_coord = VERTEX.y;
}

void fragment() {
	if (y_coord < 0.999f) 
		ALBEDO = texture(WaterTexture, UV).rgb;
    else if (y_coord > 1.001) 
		ALBEDO = texture(StoneTexture, UV).rgb;
    else 
        ALBEDO = texture(DirtTexture, UV).rgb;
}