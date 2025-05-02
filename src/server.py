# This file will read the serial from the board and send the data over to unity


# We will use the socket library to create a server
import socket
import serial
import time

# The ammount of bytes that the server will receive.
HEADER = 64  # You can change this value as you need
# The format of the messages that the server will receive.
FORMAT = "utf-8"
# The message that the server will receive to disconnect the client.
DISCONNECT_MESSAGE = "GoodBye!" # You can use whatever message, but it MUST be the same as in unity.
# The port that the server will use.
PORT = 5050 # You can change this value as you need
# The IPV4 address of your computer.
SERVER = socket.gethostbyname(socket.gethostname()) # This function gets the IP by itself.
# You can change this line by this one if you want to use a specific IP:
# SERVER = "YOUR_IP" # Change this value to your IPV4 address.
addr = (SERVER, PORT)

serial_port = "COM4"  # Change this to your serial port
baud_rate = 9600  # Change this to your baud rate

# Function to handle the Unity client.
# You can also use this function as a thread to handle multiple Unity files at the same time.
def handle_client(client, address, serial_connection):
    print(f"Client conected: {address}")
    client_connected = True
    while client_connected: # Read the messages until the client disconnects.
        # Read from the serial port and send the data to Unity
        serial_data = read_serial(serial_connection)
        if serial_data:
            # print(f"Sending to Unity: {serial_data}")
            send_message(serial_data, client)

    client.close() # Close the connection with the client.
    print(f"{address} disconnected.")

# Function to send a message to the client.
def send_message(msg, client):
    # Transform the message to bytes.
    message = msg.encode(FORMAT)
    # Send the message.
    client.send(message)

# Function to handle the message recieved from Unity.
# You can change this function to do whatever you want with the message.
def handle_message(msg):
    print(f"Message received: {msg}")
    
# Function to read from the serial port
def read_serial(serial_connection):
    if serial_connection and serial_connection.is_open:
        try:
            line = serial_connection.readline().decode('utf-8').strip()  # Read a line from the serial port
            return line
        except Exception as e:
            print(f"Error reading from serial port: {e}")
            return None
    else:
        print("Serial connection is not open.")
        return None
    

if __name__ == "__main__":
    # Connect to the serial port
    try:
        serial_connection = serial.Serial(serial_port, baud_rate, timeout=1)
        print(f"Connected to serial port {serial_port} at {baud_rate} baud.")
    except serial.SerialException as e:
        print(f"Error opening serial port: {e}")
        serial_connection = None
        exit(1)
        
    data_received = 0
    while data_received < 1:
        # Receive data from the serial port to ensure it's ready
        serial_data = read_serial(serial_connection)
        if serial_data:
            print(f"Initial serial data received: {serial_data}")
            data_received += 1
        else:
            print("Waiting for initial serial data...")
            time.sleep(1)
            
    # # Create and start the server# # 
    # Create the server.
    error = True
    while error:
        try:
            server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            server.bind(addr)
            # Start the server.
            server.listen()
            print(f"Server open on {SERVER}")
            # Wait for a client to connect.
            client, address = server.accept()
            print(f"Connection from {address} has been established!")
            
            # Handle the client.
            handle_client(client, address, serial_connection)
            # When the client disconnects, close the server.
            server.close()
            
        except KeyboardInterrupt:
            print("Server closed by user.")
            error = False
            
        except Exception as e:
            print(f"Error: {e}")
            error = True
            time.sleep(1)