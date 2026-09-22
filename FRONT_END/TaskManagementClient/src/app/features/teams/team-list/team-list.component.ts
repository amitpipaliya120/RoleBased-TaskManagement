import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TeamService } from '../../../core/services/team.service';
import { Team } from '../../../core/models/team.model';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './team-list.component.html',
  styleUrls: ['./team-list.component.scss']
})
export class TeamListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private teamService = inject(TeamService);
  private toastr = inject(ToastrService);
  teams: Team[] = [];
  teamForm: FormGroup;
  constructor() {
    this.teamForm = this.fb.group({
      teamName: ['', [Validators.required, Validators.minLength(3)]]
    });
  }
  ngOnInit(): void {
    this.loadTeams();
  }
  loadTeams() {
    this.teamService.getTeams().subscribe(t => this.teams = t);
  }
  saveTeam() {
    if (this.teamForm.invalid) {
      this.toastr.warning('Please enter a valid team name.', 'Validation Error');
      this.teamForm.markAllAsTouched();
      return;
    }
    this.teamService.createTeam(this.teamForm.value).subscribe(() => {
      this.toastr.success('Team created successfully!', 'Success');
      this.teamForm.reset();
      this.loadTeams();
    });
  }
  deleteTeam(id: number | undefined) {
    if (id && confirm('Are you sure you want to delete this team?')) {
      this.teamService.deleteTeam(id).subscribe(() => {
        this.toastr.info('Team deleted.', 'Success');
        this.loadTeams();
      });
    }
  }
}
