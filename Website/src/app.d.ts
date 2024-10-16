// See https://kit.svelte.dev/docs/types#app
// for information about these interfaces
import type bunyan from 'bunyan';

declare global {
  namespace App {
    // interface Error {}
    interface Locals {
      hasValidJwt: boolean;
      logger: bunyan.Logger;
    }
    // interface PageData {}
    // interface PageState {}
    // interface Platform {}
  }
}

export {};
