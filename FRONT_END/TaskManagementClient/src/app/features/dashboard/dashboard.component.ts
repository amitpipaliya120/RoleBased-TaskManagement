import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../core/services/task.service';
import { AuthService } from '../../core/services/auth.service';
import { Task } from '../../core/models/task.model';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  taskService = inject(TaskService);
  authService = inject(AuthService);
  tasks: Task[] = [];
  filteredTasks: Task[] = [];
  todoCount = 0;
  inProgressCount = 0;
  doneCount = 0;
  filterStatus = '';
  filterPriority = '';
  filterDeadline = '';
  ngOnInit(): void {
    this.loadTasks();
  }
  loadTasks() {
    const role = this.authService.getRole();
    if (role === 'User') {
      const userId = Number(localStorage.getItem('user_id'));
      this.taskService.getTasksByAssignee(userId).subscribe(data => {
        this.tasks = data;
        this.applyFilters();
        this.calculateMetrics();
      });
    } else {
      this.taskService.getTasks().subscribe(data => {
        this.tasks = data;
        this.applyFilters();
        this.calculateMetrics();
      });
    }
  }
  calculateMetrics() {
    this.todoCount = this.tasks.filter(t => t.status === 'To Do').length;
    this.inProgressCount = this.tasks.filter(t => t.status === 'In Progress').length;
    this.doneCount = this.tasks.filter(t => t.status === 'Done').length;
  }
  applyFilters() {
    this.filteredTasks = this.tasks.filter(t => {
      const matchStatus = this.filterStatus ? t.status === this.filterStatus : true;
      const matchPriority = this.filterPriority ? t.priority === this.filterPriority : true;
      const matchDeadline = this.filterDeadline ? (t.deadline && t.deadline.startsWith(this.filterDeadline)) : true;
      return matchStatus && matchPriority && matchDeadline;
    });
  }
}
