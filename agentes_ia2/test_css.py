import requests

try:
    print("Testing /ui/css/styles.css")
    r1 = requests.get("http://localhost:8000/ui/css/styles.css")
    print(f"Status: {r1.status_code}")
    print(f"History: {r1.history}")
except Exception as e:
    print(f"Error: {e}")
