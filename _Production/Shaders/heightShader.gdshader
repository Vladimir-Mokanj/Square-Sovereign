shader_type spatial;

uniform vec4 waterColor = vec4(0.0, 0.0, 1.0, 1.0); // Blue at the lowest point
uniform vec4 landColor = vec4(0.0, 1.0, 0.0, 1.0); // Green at the middle point
uniform vec4 hillColor = vec4(1.0, 0.0, 0.0, 1.0);  // Red at the highest point

varying float y_coord;

void vertex() {
    y_coord = VERTEX.y;
}

void fragment() {
    ALBEDO = (y_coord < 0.0f) ? waterColor.rgb : ((y_coord > 0.0f) 
							  ? hillColor.rgb : landColor.rgb);
}