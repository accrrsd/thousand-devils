

shader_type spatial;
uniform vec3 albedo : source_color = vec3(0,0.322,0.431);
uniform float metallic : hint_range(0.0, 1.0) = 0;
uniform float roughness : hint_range(0.0, 1.0) = 0.02;

group_uniforms Waves;
uniform float wave_size = 4.0;
uniform float wave_height_offset = 0.05;
uniform float wave_coverage_offset:hint_range(-1.0, 1.0, 0.1) = 0.0;
uniform vec2 wave_dir1 = vec2(1.0, 0.0);
uniform vec2 wave_dir2 = vec2(0.0, 1.0);
uniform float wave_speed : hint_range(0.0, 0.2) = 0.015;
uniform sampler2D noise_normal;
uniform sampler2D noise_normal2;
uniform sampler2D wave_texture;

varying vec3 world_pos;
varying vec3 model_view;
varying vec3 model_normal_mat;

float get_smooth_wave_pos(float edge_sensivity, float model_side_cord){
	float res = smoothstep(0.0, edge_sensivity, abs(model_side_cord));
	if (model_side_cord>0.0){
		return res;
	}
	if (model_side_cord<0.0){
		return -res;
	}
	return 0.0;
}

	//float smoothStepValue = smoothstep(-0.3, 0.3, abs(model_view.x)) * sign(model_view.x);
	// todo Проблема тут в том, что при повороте в 45 градусов, x координата (правая верхняя сторона) и левая нижняя сторона, а не левая верхняя. Это нужно пересчитать.

void vertex(){
	// global cords
	world_pos = (MODEL_MATRIX*vec4(VERTEX, 1.0)).xyz;
	//model_normal_mat = MODEL_NORMAL_MATRIX*vec3(1,1,1);
	// Извлечение угла поворота по оси x
	//float yaw = atan(MODEL_MATRIX[2][1], MODEL_MATRIX[2][0]);
	// Извлечение угла поворота по оси y
	//float pitch = atan(-MODEL_MATRIX[2][2], MODEL_MATRIX[0][2]);
	// Извлечение угла поворота по оси z
	//float roll = atan(MODEL_MATRIX[1][0], MODEL_MATRIX[0][0]);
	// local cords
	model_view = MODEL_NORMAL_MATRIX * vec3(VERTEX);
	float wave_height = texture(wave_texture, world_pos.xz / wave_size + TIME * wave_speed).r;
	float current_wave_height = wave_height*wave_height_offset;
	//todo добавить условия, чтобы применялось только когда абсолютные грани (например y сдвинут влево (45 градусов))
	if (model_view.y>wave_coverage_offset){
		//test fo expample
		//VERTEX.x += get_smooth_wave_pos(0.3, model_view.x);
		if (model_view.x>0.0){
		}	
		else if (model_view.x<0.0){
			if (dot(NORMAL.x, world_pos.x)-NORMAL.x<0.0){
				
			}
		}	
	}
	
	
		//if (model_view.x>0.0){
			//VERTEX.x+=get_smooth_wave_pos(0.3, model_view.x)*current_wave_height;
			////VERTEX.y+=get_smooth_wave_pos(0.3, model_view.y)*current_wave_height;
		//}
		//else if (model_view.x<0.0){
			//VERTEX.y+=get_smooth_wave_pos(0.3, model_view.y)*current_wave_height;			
		//}
		//VERTEX.x += smoothStepValue * current_wave_height * 10.0;
			//VERTEX.y += smoothstep(-0.3, 0.3, model_view.y)*current_wave_height;
			//VERTEX.x += get_smooth_wave_pos(0.1, model_view.x) * current_wave_height;
			//VERTEX.y += get_smooth_wave_pos(0.1, model_view.y) * current_wave_height;
}

void fragment(){
	if (model_view.y>wave_coverage_offset){
		ALBEDO = vec3(1,0,0);
		if (model_view.x>0.0){
			ALBEDO=vec3(0,1,0);
		}
		else if (model_view.x<0.0){
			ALBEDO = vec3(0,0,1);
			if (dot(model_view.x, world_pos.x)-model_view.x<0.0){
				ALBEDO=vec3(1,0,0);
			}
		}
	}
}