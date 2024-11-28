import json
import gym
import numpy as np

class MS_env(gym.Env):
    def __init__(self, observation_space_shape, action_space_shape):
        super(MS_env, self).__init__()

        self.observation_space = gym.spaces.Tuple((
            gym.spaces.Box(low=0, high=1, shape=(
                observation_space_shape,), dtype=np.float32),
        ))

        # Action discrete (select note)
        self.action_space = gym.spaces.Discrete(action_space_shape)

        # This will be accessed by the agent
        self.observation_space_shape = observation_space_shape

    def reset(self, state, actual_label):
        self.state = state
        self.actual_label = actual_label
        return self.state

    def step(self, action):
        done = False
        if (action == self.actual_label):
            reward = 100
            done = True
        else:
            reward = 0
            done = True

        return self.state, reward, done, {}
    
    def save_action_space(self):
        with open('models/action_space.json', 'w') as f:
            json.dump(self.action_space.n, f)