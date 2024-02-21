extends Node2D

@onready
var currentTexture: TextureRect = $Texture
@onready
var light: PointLight2D = $PointLight2D

@export
var ligthOnTexture: Texture2D
@export
var ligthOffTexture: Texture2D
@export
var lightColor: Color
@export
var button: Button


var lightOn: bool = false
var pulse_value = 0
var min_pulse_energy = 0.95
var max_pulse_energy = 2

func _ready():
    button.toggled.connect(_on_button_toggled)
    light.color = lightColor
    turn_off()
    # set random pulse start value
    pulse_value = randf()*10

func _process(delta):
    if(lightOn):
        pulsate(delta)

func pulsate(delta):
    pulse_value = fmod(pulse_value + delta, 2*PI)
    
    var energy = lerpf(min_pulse_energy, max_pulse_energy, abs(sin(pulse_value)))
    
    light.energy = energy

func turn_on():
    currentTexture.texture = ligthOnTexture
    light.visible = true

func turn_off():
    currentTexture.texture = ligthOffTexture
    light.visible = false

func _on_button_toggled(toggled_on):
    lightOn = !lightOn
    if(lightOn):
        turn_on()
    else:
        turn_off()
