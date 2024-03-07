extends Button

const zone_dict_plus = {"offset": 10, "speedValue": 1.5, "length": 2}
const zone_dict_minus = {"offset": 10, "speedValue": 0.5, "length": 2}
var game_progress_bar = null
func _ready():
	game_progress_bar = get_parent().get_node("GameProgressBar")
	if game_progress_bar == null:
		assert(false, "game_progress_bar is null")

func _on_Button_pressed_plus():
	game_progress_bar.value += 1

func _on_Button_pressed_minus():
	game_progress_bar.value -= 1
