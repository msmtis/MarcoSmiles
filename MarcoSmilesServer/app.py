from flask import Flask, jsonify, request
from utils.utils import load_action_space, read_request, toint
from DQN.Double_DQN import Network

app = Flask(__name__)

@app.route('/hello-world', methods=['GET'])
def hello_world():
    return jsonify({"message": "hello world!"})


@app.route('/hand-data', methods=['POST'])
def hand_data():
    try:
        global network
        predictions = []

        hand_data, note = read_request(request.json)
        print("Note received: " + str(note))

        # Predict whole batch
        for pose in hand_data:
            prediction = network.learn(pose, note)
            predictions.append(toint(prediction))

        # Get the most common prediction
        prediction = max(set(predictions), key=predictions.count)
        print("Predicted: " + str(prediction))

        # Live accuracy logic
        network.predicted_counter += 1
        if prediction == note:
            network.guessed_counter += 1

        # Bye bye
        return jsonify({
            "message": f"{prediction}",
            "accuracy": int(((100/network.predicted_counter)*network.guessed_counter) if network.predicted_counter > 0 else 0)
        })
    except Exception as e:
        print(e)
        return jsonify({
            "message": "Error processing request"}), 500


@app.route('/hand-data-play-mode', methods=['POST'])
def hand_data_play_mode():
    try:
        global network
        predictions = []

        hand_data, _ = read_request(request.json)

        # Predict whole batch
        for pose in hand_data:
            prediction = network.agent.get_action(pose, training_mode=False)
            predictions.append(toint(prediction))

        # Get the most common prediction
        prediction = max(set(predictions), key=predictions.count)
        print("Predicted: " + str(prediction) + " with confidence: " + str(predictions.count(prediction) / len(predictions)))
        
        # Check if prediction has at least 55% confidence
        if predictions.count(prediction) / len(predictions) < 0.65:
            prediction = "_"
            

        # Bye bye
        return jsonify({
            "message": f"{prediction}"
        })
    except Exception as e:
        print(e)
        return jsonify({
            "message": "Error processing request"}), 500


@app.route('/save-model', methods=['GET'])
def end_training():
    try:
        global network
        network.save_model()
        network.save_target_model()
        network.save_predicted_and_guessed()
        network.env.save_action_space()
        return jsonify({"message": "Network saved"}), 200
    except Exception as e:
        print(e)
        return jsonify({"message": "Error saving network"}), 500


@app.route('/new-model', methods=['POST'])
def new_model():
    try:
        global network
        output_dimension: int = request.json.get('output_dimension', None)
        network = Network(
            action_space_shape=output_dimension) if output_dimension is not None else Network()
        network.save_model()
        network.save_target_model()
        network.save_predicted_and_guessed()
        network.env.save_action_space()
        return jsonify({"message": "New model created"})
    except Exception as e:
        print(e)
        return jsonify({"message": "Error creating new model"}), 500


if __name__ == '__main__':
    action_space_shape_dim = load_action_space()
    network = Network(
        action_space_shape=action_space_shape_dim) if action_space_shape_dim is not None else Network()
    network.load_model()
    network.load_target_model()
    network.load_predicted_and_guessed()
    app.run(host='0.0.0.0', port=5005)
