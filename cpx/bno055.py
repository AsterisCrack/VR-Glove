import time
import board
import busio
import struct

# Setup I2C
i2c = busio.I2C(board.SCL, board.SDA)

# BNO055 I2C address
BNO055_ADDRESS = 0x28

# BNO055 register addresses
QUATERNION_DATA_W_LSB = 0x20

# Helper: Read quaternion
def read_quaternion():
    result = bytearray(8)
    while not i2c.try_lock():
        pass
    try:
        i2c.writeto(BNO055_ADDRESS, bytes([QUATERNION_DATA_W_LSB]))
        i2c.readfrom_into(BNO055_ADDRESS, result)
    finally:
        i2c.unlock()
    
    w, x, y, z = struct.unpack('<hhhh', result)
    w /= 16384.0
    x /= 16384.0
    y /= 16384.0
    z /= 16384.0
    
    return w, x, y, z


# Initialize sensor (set operation mode to NDOF mode)
def initialize_bno055():
    while not i2c.try_lock():
        pass  # Wait until we have the lock
    try:
        # Set BNO055 to CONFIGMODE
        i2c.writeto(BNO055_ADDRESS, bytes([0x3D, 0x00]))
        time.sleep(0.025)
        # Set BNO055 to NDOF mode (Fusion mode)
        i2c.writeto(BNO055_ADDRESS, bytes([0x3D, 0x0C]))
        time.sleep(0.02)
    finally:
        i2c.unlock()