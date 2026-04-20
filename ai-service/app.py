from fastapi import FastAPI, UploadFile, File
import pdfplumber
from sentence_transformers import SentenceTransformer
import faiss
import numpy as np
from pydantic import BaseModel
from groq import Groq
import os

app = FastAPI()

# 🔥 Groq client
client = Groq(api_key=os.getenv("GROQ_API_KEY"))

# 🔥 Embedding model
model = SentenceTransformer("all-MiniLM-L6-v2")

# 🔥 Global storage
stored_chunks = []
faiss_index = None


@app.get("/")
def home():
    return {"message": "AI Service Running 🔥"}


# 🔥 Helper function
def chunk_text(text, chunk_size=500, overlap=100):
    chunks = []
    start = 0

    while start < len(text):
        end = start + chunk_size
        chunk = text[start:end]
        chunks.append(chunk)
        start += chunk_size - overlap

    return chunks


@app.post("/process-pdf")
async def process_pdf(file: UploadFile = File(...)):
    global stored_chunks, faiss_index

    text = ""

    with pdfplumber.open(file.file) as pdf:
        for page in pdf.pages:
            text += page.extract_text() or ""

    chunks = chunk_text(text)

    embeddings = model.encode(chunks)
    embeddings = np.array(embeddings).astype("float32")

    dimension = embeddings.shape[1]
    index = faiss.IndexFlatL2(dimension)
    index.add(embeddings)

    stored_chunks = chunks
    faiss_index = index

    return {
        "total_chunks": len(chunks),
        "vector_dimension": dimension
    }


class QueryRequest(BaseModel):
    question: str


@app.post("/ask")
async def ask_question(request: QueryRequest):
    global stored_chunks, faiss_index

    if faiss_index is None:
        return {"error": "No PDF processed yet"}

    # 🔥 Step 1: embedding
    query_embedding = model.encode([request.question])
    query_embedding = np.array(query_embedding).astype("float32")

    # 🔥 Step 2: search
    distances, indices = faiss_index.search(query_embedding, k=3)
    results = [stored_chunks[i] for i in indices[0]]

    # 🔥 Step 3: context
    context = "\n".join(results)

    try:
        # 🔥 Groq LLM call
        response = client.chat.completions.create(
           model="llama3-8b-8192",
            messages=[
                {
                    "role": "system",
                    "content": "Answer only using the given context. If not found, say 'Not in document'."
                },
                {
                    "role": "user",
                    "content": f"Context:\n{context}\n\nQuestion: {request.question}"
                }
            ]
        )

        answer = response.choices[0].message.content

        return {
            "question": request.question,
            "answer": answer
        }

    except Exception as e:
        return {
            "error": "Groq API failed",
            "details": str(e)
        }