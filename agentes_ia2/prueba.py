import os
from openai import OpenAI
from dotenv import load_dotenv

load_dotenv()

api_key = os.getenv("OPENAI_API_KEY")

print("API KEY:", api_key)

client = OpenAI(api_key=api_key)

response = client.responses.create(
    model="gpt-5.2",
    input="Dame una frase inteligente"
)

print(response.output[0].content[0].text)