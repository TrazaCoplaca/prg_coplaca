from database.sqlserver import get_schema, schema_to_text

schema = get_schema()

print(schema)

print("\n--- ESQUEMA ---")

print(schema_to_text(schema))