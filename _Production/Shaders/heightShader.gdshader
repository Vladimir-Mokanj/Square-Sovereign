//uniform vec4 waterColor = vec4(0.0, 0.0, 1.0, 1.0); // Blue at the lowest point
//uniform vec4 landColor = vec4(0.0, 1.0, 0.0, 1.0); // Green at the middle point
//uniform vec4 hillColor = vec4(1.0, 0.0, 0.0, 1.0);  // Red at the highest point

shader_type spatial;

uniform sampler2D waterTexture;
uniform sampler2D dirtTexture;
uniform sampler2D stoneTexture;
uniform float cellSize;

varying float y_coord;

void vertex() {
    y_coord = VERTEX.y;
}

void fragment() {
	if (y_coord < cellSize - 0.01f) 
		ALBEDO = texture(waterTexture, UV).rgb;
    else if (y_coord > cellSize + 0.01f) 
		ALBEDO = texture(stoneTexture, UV).rgb;
    else 
        ALBEDO = texture(dirtTexture, UV).rgb;
}