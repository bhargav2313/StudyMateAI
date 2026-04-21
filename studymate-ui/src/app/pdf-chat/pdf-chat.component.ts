import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pdf-chat',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './pdf-chat.component.html',
  styleUrls: ['./pdf-chat.component.css']
})
export class PdfChatComponent {

  selectedFile: File | null = null;
  question: string = '';
  loading: boolean = false;

  chatHistory: { question: string, answer: string }[] = [];

  // 🔥 BASE URL (change if needed)
  apiUrl = 'http://localhost:5112';  

  constructor(private http: HttpClient) {}

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  upload() {
    if (!this.selectedFile) {
      alert("Select file first");
      return;
    }

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    console.log("📤 Uploading to:", `${this.apiUrl}/api/pdf/process`);

    this.http.post(`${this.apiUrl}/api/pdf/process`, formData)
      .subscribe({
        next: (res) => {
          console.log("✅ Upload success:", res);
          alert("PDF processed ✅");
        },
        error: (err) => {
          console.error("❌ Upload error:", err);
          alert("Upload failed ❌ - check console");
        }
      });
  }

  ask() {
    if (!this.question) return;

    const userQuestion = this.question;
    this.loading = true;

    console.log("📨 Asking:", userQuestion);

    this.http.post(`${this.apiUrl}/api/pdf/ask`, {
      question: userQuestion
    }).subscribe({
      next: (res: any) => {
        console.log("✅ Answer:", res);

        this.chatHistory.push({
          question: userQuestion,
          answer: res.answer
        });

        this.question = '';
        this.loading = false;
      },
      error: (err) => {
        console.error("❌ Ask error:", err);

        this.chatHistory.push({
          question: userQuestion,
          answer: "Error ❌"
        });

        this.loading = false;
      }
    });
  }
}