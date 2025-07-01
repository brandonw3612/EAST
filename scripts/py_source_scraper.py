from pathlib import Path
from itertools import islice
from datasets import load_dataset

ocr2_dataset = load_dataset("nvidia/OpenCodeReasoning-2", split="python", streaming=True)

solutions = []

for i, item in enumerate(islice(ocr2_dataset, 1000)):
    solutions.append({
        "id": item["id"],
        "solution": item["solution"]
    })

for i, item in enumerate(solutions):
    sol_id = item["id"]
    solution = item["solution"]
    file_path = Path(f"../samples/python/{sol_id}/{sol_id}.py")
    if file_path.exists():
        print(f"File {file_path} already exists, skipping.")
        continue
    file_path.parent.mkdir(parents=True, exist_ok=True)
    with file_path.open("w", encoding="utf-8") as f:
        f.write(solution)