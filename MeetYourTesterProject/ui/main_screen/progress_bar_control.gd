extends Control

@onready var terminal = $"../Terminal/_terminal_mock/terminal_content"
const PROGRESS_BAR_DICTIONARY_KEY = "progress_bar"
var zones_scene = preload("res://ui/main_screen/progress_bar_zones_scene.tscn")
var has_nodo_spawned:bool = false
var begin_nodo: int
var end_nodo: int
 
func _ready():
	terminal.connect("answer_signal", apply_progress_bar_effects)

func _process(delta):
	if has_nodo_spawned:
		var position_bar = get_pixel_from_percent($GameProgressBar.value , $GameProgressBar.size['x'])
		if position_bar > begin_nodo && position_bar < end_nodo:
			$GameProgressBar.texture_progress = load("res://images/main-game/progress-bar/yellow-bars-faster.svg")
		if position_bar > end_nodo:
				remove_zone()
			

func apply_progress_bar_effects(answer_impact: Dictionary):
	if PROGRESS_BAR_DICTIONARY_KEY == answer_impact["type"]:
		print("Answer Selected!")
		var answerEffects = answer_impact["effects"]
		
		if answerEffects.has("zone"):
			create_zone(answerEffects)

		# Godot handle under the hood the check for progress bar boundaries. If you add 1000 with a max value of 100 it will be 100.
		$GameProgressBar.value += answerEffects.value
		# TODO
		#var speed_value = zone_effects.get("speedValue",0)
		#if speed_value > 5 and speed_value < 10: 
		
func get_pixel_from_percent(percent: float, total: int) -> int:
	return int(percent * total / 100)
		
func create_zone(answer_effects: Dictionary):
	print(answer_effects)
	var new_zone_scene = zones_scene.instantiate()
	var new_zone_node:TextureRect = new_zone_scene.get_child(0)
	# https://www.davcri.it/posts/godot-reparent-node/
	new_zone_scene.remove_child(new_zone_node)
	var zone_effects:Dictionary = answer_effects.get("zone",{})
	
	var zone_length = int(zone_effects.get("length",1))
	match zone_length: 		# set zone texture based on length (sm,md,lg)
		Globals.progress_bar_zone_small:
			new_zone_node.texture = preload("res://images/main-game/progress-bar/blue-zone-sm.svg")
		Globals.progress_bar_zone_medium:
			new_zone_node.texture = preload("res://images/main-game/progress-bar/blue-zone-md.svg")
		Globals.progress_bar_zone_large:
			new_zone_node.texture = preload("res://images/main-game/progress-bar/blue-zone-lg.svg")
		_:
			print("Unrecognized zone length: " + str(zone_length))
			
	new_zone_node.offset_left = zone_effects.get("offset",0) + get_pixel_from_percent($GameProgressBar.value , $GameProgressBar.size['x'])	# set offset from left of progress bar

	print($GameProgressBar.value )
	$ZonesContainer.add_child(new_zone_node)
	has_nodo_spawned = true
	begin_nodo = new_zone_node.offset_left
	end_nodo = new_zone_node.texture.get_width() + begin_nodo
	

func remove_zone():
	var nodo = $ZonesContainer.get_child(0)
	$ZonesContainer.remove_child(nodo)
	begin_nodo = -1
	end_nodo = 9223372036854775807 #max int in godot
	has_nodo_spawned = false
	$GameProgressBar.texture_progress = load("res://images/main-game/progress-bar/yellow-bars-neutral.svg")
