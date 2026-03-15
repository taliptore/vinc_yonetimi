import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'providers/auth_provider.dart';
import 'screens/login_screen.dart';
import 'screens/home_screen.dart';
import 'screens/job_list_screen.dart';
import 'screens/job_start_screen.dart';
import 'screens/breakdown_screen.dart';

void main() {
  runApp(const VincYonetimApp());
}

class VincYonetimApp extends StatelessWidget {
  const VincYonetimApp({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (_) => AuthProvider()..init(),
      child: MaterialApp(
        title: 'Vinç Yönetim',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: const Color(0xFF1e3a5f)),
          useMaterial3: true,
        ),
        initialRoute: '/',
        routes: {
          '/': (context) => const AuthWrapper(),
          '/login': (context) => const LoginScreen(),
          '/home': (context) => const HomeScreen(),
          '/jobs': (context) => const JobListScreen(),
          '/job_start': (context) {
            final args = ModalRoute.of(context)?.settings.arguments as Map<String, dynamic>?;
            return JobStartScreen(job: args);
          },
          '/breakdown': (context) => const BreakdownScreen(),
        },
      ),
    );
  }
}

class AuthWrapper extends StatelessWidget {
  const AuthWrapper({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<AuthProvider>(
      builder: (_, auth, __) {
        if (auth.loading) return const Scaffold(body: Center(child: CircularProgressIndicator()));
        if (auth.isLoggedIn) return const HomeScreen();
        return const LoginScreen();
      },
    );
  }
}
