import { Component, inject, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { UserService } from '../../../core/services/user.service';
import { CommentService } from '../../../core/services/comment.service';
import { Task } from '../../../core/models/task.model';
import { User } from '../../../core/models/user.model';
import { Comment } from '../../../core/models/comment.model';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private taskService = inject(TaskService);
  public authService = inject(AuthService);
  private userService = inject(UserService);
  private commentService = inject(CommentService);
  private toastr = inject(ToastrService);
  @ViewChild('addTaskModal') addTaskModal!: ElementRef<HTMLDialogElement>;
  tasks: Task[] = [];
  users: User[] = [];
  selectedTask: Task | null = null;
  taskComments: Comment[] = [];
  taskForm: FormGroup;
  commentForm: FormGroup;
  constructor() {
    this.taskForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      status: ['To Do', Validators.required],
      priority: ['Medium', Validators.required],
      assigneeId: [null],
      deadline: ['', Validators.required]
    });
    this.commentForm = this.fb.group({
      commentText: ['', [Validators.required, Validators.minLength(3)]]
    });
  }
  ngOnInit(): void {
    this.loadTasks();
    this.loadUsersForAssignment();
  }
  canManageTasks(): boolean {
    const role = this.authService.getRole();
    return role === 'Admin' || role === 'Manager';
  }
  loadTasks() {
    if (this.canManageTasks()) {
      this.taskService.getTasks().subscribe(t => this.tasks = t);
    } else {
      const userId = Number(localStorage.getItem('user_id'));
      this.taskService.getTasksByAssignee(userId).subscribe(t => this.tasks = t);
    }
  }
  loadUsersForAssignment() {
    if (!this.canManageTasks()) return;
    const role = this.authService.getRole();
    if (role === 'Manager') {
      this.userService.getMyUsers().subscribe(u => this.users = u);
    } else {
      this.userService.getUsers().subscribe(u => this.users = u);
    }
  }
  startCreate() {
    this.selectedTask = null;
    this.taskForm.reset({ status: 'To Do', priority: 'Medium' });
    this.addTaskModal.nativeElement.showModal();
  }
  cancelCreate() {
    this.taskForm.reset();
    this.addTaskModal.nativeElement.close();
  }
  saveTask() {
    if (this.taskForm.invalid) {
      this.toastr.warning('Please fill all required fields correctly.', 'Validation Error');
      this.taskForm.markAllAsTouched();
      return;
    }
    this.taskService.createTask(this.taskForm.value).subscribe(() => {
      this.toastr.success('Task created successfully!', 'Success');
      this.addTaskModal.nativeElement.close();
      this.loadTasks();
    });
  }
  updateStatus(task: Task, newStatus: string) {
    if (task.taskId) {
      this.taskService.updateTaskStatus(task.taskId, newStatus).subscribe({
        next: () => {
          this.toastr.info(`Task status updated to ${newStatus}`);
          this.loadTasks();
        },
        error: (err) => {
          this.toastr.error('Failed to update task status');
          console.error(err);
          this.loadTasks();
        }
      });
    }
  }
  @ViewChild('deleteTaskModal') deleteTaskModal!: ElementRef<HTMLDialogElement>;
  taskToDelete: number | null = null;
  confirmDeleteTask(id: number | undefined) {
    if (id) {
      this.taskToDelete = id;
      this.deleteTaskModal.nativeElement.showModal();
    }
  }
  executeDeleteTask() {
    if (this.taskToDelete) {
      this.taskService.deleteTask(this.taskToDelete).subscribe(() => {
        this.toastr.success('Task deleted.', 'Success');
        this.deleteTaskModal.nativeElement.close();
        this.taskToDelete = null;
        this.loadTasks();
      });
    }
  }
  @ViewChild('viewTaskModal') viewTaskModal!: ElementRef<HTMLDialogElement>;
  @ViewChild('commentsModal') commentsModal!: ElementRef<HTMLDialogElement>;
  viewTaskDetails(task: Task) {
    this.selectedTask = task;
    this.viewTaskModal.nativeElement.showModal();
  }
  closeViewTask() {
    this.viewTaskModal.nativeElement.close();
    this.selectedTask = null;
  }
  openComments(task: Task) {
    this.selectedTask = task;
    if (task.taskId) {
      this.commentService.getCommentsByTask(task.taskId).subscribe(c => {
        this.taskComments = c;
        this.commentsModal.nativeElement.showModal();
      });
    }
  }
  closeComments() {
    this.commentsModal.nativeElement.close();
    this.selectedTask = null;
  }
  addComment() {
    if (this.commentForm.invalid || !this.selectedTask?.taskId) {
      this.commentForm.markAllAsTouched();
      return;
    }
    const newComment: Comment = {
      taskId: this.selectedTask.taskId,
      commentText: this.commentForm.value.commentText
    };
    this.commentService.createComment(newComment).subscribe(() => {
      this.toastr.success('Comment added!', 'Success');
      this.commentForm.reset();
      if (this.selectedTask?.taskId) {
        this.commentService.getCommentsByTask(this.selectedTask.taskId).subscribe(c => this.taskComments = c);
      }
    });
  }
}
