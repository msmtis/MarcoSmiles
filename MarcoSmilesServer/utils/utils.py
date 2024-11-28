import json
import numpy as np

def read_request(request_as_json):
    left_hand_data = request_as_json.get('LeftHandWrappers')
    right_hand_data = request_as_json.get('RightHandWrappers')
    note = request_as_json.get('Note', None)
    # All poses
    all_poses = []

    # Read the json data from left hand
    for pose in left_hand_data:
        numerical_values = []
        for joint in pose:
            # Position
            positions = pose[joint]
            for coordinate in positions:
                numerical_values.append(positions[coordinate])
        all_poses.append(numerical_values)
        
    # Read the json data from right hand
    for pose in right_hand_data:
        numerical_values = []
        for joint in pose:
            # Position
            positions = pose[joint]
            for coordinate in positions:
                numerical_values.append(positions[coordinate])
        all_poses.append(numerical_values)

    # print(f"Received data: {numerical_values}")
    return all_poses, note


# write request to file
def write_request(request_as_json):
    with open('tmp.json', 'a') as f:
        json.dump(request_as_json, f)

def toint(x):
    if isinstance(x, np.int64):
        return x.tolist()
    else:
        return x

# retrive action_space from file
def load_action_space():
    try:
        with open('models/action_space.json', 'r') as f:
            return json.load(f)
    except Exception as e:
        print(e)
