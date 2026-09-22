export interface User {
  userId?: number;
  name?: string;
  email?: string;
  passwordHash?: string;
  roleId?: number;
  roleName?: string;
  teamId?: number;
  managerId?: number;
  managerName?: string;
  isActive?: boolean;
}
