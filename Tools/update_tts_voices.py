#!/usr/bin/env python3
import requests
import os
import sys

if not "TTS_API_URL" in os.environ:
    print(f"TTS_API_URL is not set")
    sys.exit(-1)
    pass

if not "TTS_AUTH_TOKEN" in os.environ:
    print(f"TTS_AUTH_TOKEN is not set")
    sys.exit(-1)
    pass

url = os.environ["TTS_API_URL"]
auth_token = os.environ["TTS_AUTH_TOKEN"]

resp = requests.get(url, headers={
    "Authorization": f"Bearer {auth_token}",
})

if not resp.ok:
    print(f"Error: {resp.text}")
    sys.exit(-1)
    pass

j = resp.json()

with open("Resources/Prototypes/Adventure/tts-voices.yml", "w") as f:
    for voice in j["voices"]:
        f.write(f"""- type: ttsVoice
  name: {voice['name']}
  description: {voice['description']}
  speaker: {voice['speakers'][0]}
  id: {voice['speakers'][0]}

""")
        print(voice)
        pass
    pass
