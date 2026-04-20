from fastapi import FastAPI, UploadFile, File
import pdfplumber

app = FastAPI()

@app.get("/")
def home():
    return {"message": "AI Service Running 🔥"}

@app.post("/process-pdf")
async def process_pdf(file: UploadFile = File(...)):
    text = ""

    with pdfplumber.open(file.file) as pdf:
        for page in pdf.pages:
            text += page.extract_text() or ""

    return {"text": text[:1000]}