# Escribe tu código aquí :-)
import time
import board
import busio
import analogio
import bno055 as bno

# Threshold for touch sensor
TOUCH_THRESHOLD = 1700  # Adjust this value based on your touch sensor's sensitivity
pressure_pad = analogio.AnalogIn(board.A3)
last_values_pressure = []
n_pressure = 5 # Number of values to average

# Initialize BNO055
bno.initialize_bno055()

# Initialize flex sensor variables
MIN_FLEX = 0  # Minimum flex value (adjust as needed)
MAX_FLEX = 65535  # Maximum flex value (adjust as needed)
flex_pins = [analogio.AnalogIn(pin) for pin in [board.A2, board.A1, board.A0, board.A7, board.A6]]
last_values_flex = [[] for _ in range(len(flex_pins))]
n_flex = 5  # Number of values to average for flex sensors

filter_size = 1  # Number of samples for averaging
# Initialize variables for averaging
history_x = []
history_y = []
history_z = []
history_w = []
history_height = []

def get_flex(pin, last_values, n=5):
    raw_value = pin.value
    #last_values.append(raw_value)
    filtered_value = filter_data(raw_value, last_values, n)  # Update the history for averaging
    # Normalize the value between 0 and 1
    #normalized_value = (filtered_value - MIN_FLEX) / (MAX_FLEX - MIN_FLEX)
    # return max(0, min(1, normalized_value))  # Ensure the value is clamped between 0 and 1
    return filtered_value

def get_touch(pin, last_values, n=2):
    voltage = pin.value
    # last_values.append(voltage)
    filtered_voltage = filter_data(voltage, last_values, n)  # Update the history for averaging
    return filtered_voltage < TOUCH_THRESHOLD  # Return True if touched, False otherwise
    # return filtered_voltage  # Return the filtered voltage value

def filter_data(data, history, history_size=10):
    if len(history) >= history_size:
        history.pop(0)
    history.append(data)  # Add new data
    return sum(history) / len(history)  # Return average

while True:
    # x, y, z, w = sensor.quaternion
    # h = sensor.height
    x, y, z, w = bno.read_quaternion()
    h = 0

    # Read flex sensors
    flex_values = [get_flex(pin, last_values_flex[i], n_flex) for i, pin in enumerate(flex_pins)]

    touch = get_touch(pressure_pad, last_values_pressure, n_pressure)

    # Update history for averaging
    filtered_x = filter_data(x, history_x)
    filtered_y = filter_data(y, history_y)
    filtered_z = filter_data(z, history_z)
    filtered_w = filter_data(w, history_w)
    filtered_h = filter_data(h, history_height)

    # Format and send data
    data_str = f"{filtered_x:.2f},{filtered_y:.2f},{filtered_z:.2f},{filtered_w:.2f},{filtered_h:.2f}," + ",".join([f"{val:.2f}" for val in flex_values]) + f",{int(touch)}\n"
    # print((int(touch),))
    print(data_str)  # Print the data string for debugging
    # print((pressure_pad.value,))
    # print((flex_values[0], flex_values[1], flex_values[2], flex_values[3], flex_values[4]),)  # Print the data string for debugging
    # print((flex_values[4],))
    # print(f"{filtered_x:.2f},{filtered_y:.2f},{filtered_z:.2f},{filtered_w:.2f},{filtered_h:.2f},")
    time.sleep(0.1)  # Send data at 100 Hz
