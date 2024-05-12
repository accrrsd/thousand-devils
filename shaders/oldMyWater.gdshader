// NOTE: Shader automatically converted from Godot Engine 4.2.2.stable.mono's StandardMaterial3D.

shader_type spatial;
render_mode cull_disabled;

uniform vec3 albedo : source_color = vec3(0,0.322,0.431);
uniform float metallic : hint_range(0.0, 1.0) = 0;
uniform float roughness : hint_range(0.0, 1.0) = 0.02;

uniform float offset : hint_range(-1.0, 1.0) = 0.5;
uniform float fade : hint_range(0.0, 1.0) = 0.1;

group_uniforms Waves;
uniform sampler2D noise_normal;
uniform sampler2D noise_normal2;
uniform sampler2D wave_texture; // todo replace to wave 
uniform float wave_size = 4.0;
uniform float wave_amount_offset = 0.0;
uniform float wave_offset = 0.05;
uniform vec2 wave_dir1 = vec2(1.0, 0.0);
uniform vec2 wave_dir2 = vec2(0.0, 1.0);
uniform float wave_speed : hint_range(0.0, 0.2) = 0.015;

varying vec3 world_pos;
varying vec3 model_view;
varying vec3 vertical_sides_pos;
varying vec3 vertex_normal;

	// set upper sides
	//dot_for_upper_sides = dot(vertical_sides_pos, NORMAL);
	
	//vec3 up_vector_viewspace = mat3(VIEW_MATRIX) * vec3(0.0, 1.0, 0.0);
	//float dot_product = dot(up_vector_viewspace, NORMAL);
	//float mask = smoothstep(offset - fade, offset + fade, dot_product);	
	
	//todo try to apply it only for upper sides
//============================================
	//Works good in old mechanic
	//float smooth_x = get_smooth_vertical_pos(0.2, vertical_sides_pos.x);
	//float smooth_y = get_smooth_vertical_pos(0.2, vertical_sides_pos.y);
	//float smooth_z = get_smooth_vertical_pos(0.2, vertical_sides_pos.z);
	//VERTEX.x += smooth_x * current_wave_height;
	//VERTEX.y -= smooth_y * current_wave_height;
	//VERTEX.z += smooth_z * current_wave_height;
	
//vec3 calculateFaceNormal(vec3 vertexNormal) {
  //if (vertexNormal.x > 0.0) {
    //return vec3(1.0, 0.0, 0.0); //X+
  //} else if (vertexNormal.x < 0.0) {
    //return vec3(-1.0, 0.0, 0.0); //X-
  //} else if (vertexNormal.y > 0.0) {
    //return vec3(0.0, 1.0, 0.0); //Y+
  //} else if (vertexNormal.y < 0.0) {
    //return vec3(0.0, -1.0, 0.0); //Y-
  //} else if (vertexNormal.z > 0.0) {
    //return vec3(0.0, 0.0, 1.0); //Z+
  //} else {
    //return vec3(0.0, 0.0, -1.0); //Z-
  //}
//}


float get_smooth_vertical_pos(float edge_sensivity, float vertical_side_cord){
	if (vertical_side_cord>0.0){
		return smoothstep(0, edge_sensivity, vertical_side_cord);
	}
	if (vertical_side_cord<-0.0){
		return -smoothstep(0, -edge_sensivity, vertical_side_cord);
	}
	return 0.0;
}

void vertex(){
	vertical_sides_pos = MODEL_NORMAL_MATRIX * vec3(0.0, 1.0, 0.0);
	// global position
	world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	// same as global positon but in model only
	model_view = MODEL_NORMAL_MATRIX * vec3(VERTEX);
	vertex_normal = NORMAL;
	
	// Vertext displacement for waves
	//Можно написать что-то типа синусоиды, которая будет измениять высоту волны (подсмотреть у крутого шейдера)
	float wave_height = texture(wave_texture, world_pos.xz / wave_size + TIME * wave_speed).r;
	float current_wave_height = wave_height*wave_offset;
	//Only upper side
	if (model_view.y>wave_amount_offset){
		VERTEX.x += get_smooth_vertical_pos(0.1, model_view.x) * current_wave_height;
		VERTEX.y += get_smooth_vertical_pos(0.1, model_view.y) * current_wave_height;
		//VERTEX.z += get_smooth_vertical_pos(0.5, model_view.z) * current_wave_height;
		
		//VERTEX.x += model_view.x * current_wave_height;
		//VERTEX.y += model_view.y * current_wave_height;
		//VERTEX.z += model_view.z * current_wave_height;
		
		
	}
	//VERTEX.x = smoothstep(-current_wave_height, +)
	//if (model_view.y>0.0){
		//// left side (relative z+ cord)
		//if (model_view.x>0.0){
		//}
		//// right side (relative z+ cord)
		//if (model_view.x<0.0){
		//}
	//}
	//if (model_view.y>0.0 && vertical_sides_pos.y>0.1){
		//VERTEX.y-=current_wave_height;
	//}
	

}

void fragment(){
	// ======================================= strange works!
	//vec3 ws_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	//ALBEDO = vec3(ws_pos.y);
	//======================================== base works!
	// also works, but different
	//vec3 vs_pos = mat3(VIEW_MATRIX) * vec3(0.0, 1.0, 0.0);
	//ALBEDO = vec3(dot(vs_pos, NORMAL));
	//======================================== mix method works
	//vec3 vs_pos = mat3(VIEW_MATRIX) * vec3(0.0, 1.0, 0.0);
	//float dot_product = dot(vs_pos, NORMAL);
	//float mask = smoothstep(offset - fade, offset + fade, dot_product);
	//// it also will work
	////float mask = smoothstep(offset, offset, dot_product);	
	//ALBEDO = mix(vec3(1,1,1), vec3(0,0,0), mask);
	//======================================== (if method) works
	//vec3 vm_v_pos = mat3(VIEW_MATRIX) * vec3(0.0, 1.0, 0.0);
	//float dot_product = dot(vm_v_pos, NORMAL);
	//if (dot_product>0.2){
		//ALBEDO = vec3(1,0,0);
	//}
	//========================================
	if (model_view.y>wave_amount_offset){
		ALBEDO = albedo;
	}
		//Only upper side
	//if (model_view.y>0.0){
		//ALBEDO = vec3(1,0,0);
		//// left side (relative z+ cord)
		//if (model_view.x>0.0){
			//if (vertex_normal.x>0.0){
			//ALBEDO = vec3(0,1,0);
				//
			//}
		//}
		//// right side (relative z+ cord)
		//if (model_view.x<0.0){
		//}
	//}
}
